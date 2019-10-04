﻿using System;
using System.Collections;
using System.Text;
using System.IO;
using DataLayer;
using System.Diagnostics;

namespace BranchAndPriceAlgorithm
{
    public class ColumnGenration
    {

        //output
        public int TotalIntVar;
        public int TotalContinuesVar;
        public double AvariageValue;
        public double AverageImprovement;
        public double AverageReducedCost;
        public double AverageAddedReducedCost;
        public int Iteration;
        public double BestSolution;
        public double elaps_time;
        public string model_status;
        public long master_Time;
        public long subMIP_time;
        public int count_MIP;
        public bool isMIP;
        public long writingX;

        public MasterProblem RMP;
        SubProblem SPMIP;
        public double[] dual;
        public bool isNodeFeasible;


        public int Interns;
        public int Disciplins;
        public int Hospitals;
        public int Timepriods;
        public int TrainingPr;
        public int Wards;
        public int Regions;
        public int DisciplineGr;
        public AllData data;

        public ColumnGenration(DataLayer.AllData allData, string insName)
        {
            data = allData;
            initial();
            SolveCG(insName);
        }

        public void SolveCG(string insName)
        {
            
            double lagrangian_LB;
            double ben = -1;
            subMIP_time = 0;
          

            RMP = new MasterProblem(data, insName);

            BestSolution = RMP.RMP.ObjValue;


            //rmp solve himself in structor
            dual = RMP.pi;
            display_dual(dual, insName);
            Stopwatch cgTime = new Stopwatch();
            cgTime.Start();

            Console.WriteLine("**********this the cost: {0}*********", BestSolution);


            int counter = 0;
            while (SolveSubProblem(dual, insName))
            {
                Iteration++;
                Stopwatch sw1 = new Stopwatch();
                counter++;
                RMP.solveRMP(insName);
                sw1.Stop();
                master_Time += sw1.ElapsedMilliseconds;
                sw1.Reset();
                double whole_time = master_Time + subMIP_time;
                dual = RMP.pi;
                display_dual(dual,insName);
                BestSolution = RMP.Benefit;
                Console.WriteLine("**********this the cost: {0}*********", RMP.Benefit);
                ////BandBMSS tmp1 = new BandBMSS(dual);
                Console.WriteLine(cgTime.ElapsedMilliseconds / 1000);
                if (cgTime.ElapsedMilliseconds / 1000 > data.AlgSettings.MasterTime)
                {
                    //break;
                }
            }
            AvariageValue = RMP.AvariageValue / (RMP.TotalContinuesVar + RMP.TotalIntVar);
            TotalContinuesVar = RMP.TotalContinuesVar;
            TotalIntVar = RMP.TotalIntVar;
            AverageImprovement /= Iteration;
            AverageReducedCost = RMP.AverageReducedCost / (RMP.DataColumn.Count - (RMP.TotalContinuesVar + RMP.TotalIntVar));
            AverageAddedReducedCost = RMP.AverageAddedReducedCost / RMP.DataColumn.Count;
            isMIP = RMP.is_mip;

            display_time(insName);

            RMP.RMP.End();
           
        }


        public void initial()
        {
            Interns = data.General.Interns;


            Disciplins = data.General.Disciplines;
            Wards = data.General.HospitalWard;
            Regions = data.General.Region;
            DisciplineGr = data.General.DisciplineGr;

            Hospitals = data.General.Hospitals;
            Timepriods = data.General.TimePriods;
            TrainingPr = data.General.TrainingPr;
        }
        public void display_dual(double[] dual, string insName)
        {
            int tmp1, tmp2;
            int[] cons = {
                Interns , Timepriods * Regions , Timepriods * Wards * Hospitals,Timepriods * Wards * Hospitals,  Interns * TrainingPr
            };

            tmp1 = 0;
            tmp2 = 0;
            using (StreamWriter tw = new StreamWriter(data.allPath.OutPutGr + insName + "Dual.txt", true))
            {

                for (int i = 0; i < dual.Length; i++)
                {
                    if (tmp2 == i)
                    {
                        //Console.WriteLine("******{0}th Equation set********", tmp1 + 2);
                        tw.WriteLine("******{0}th Equation set********", tmp1 + 2);
                        tmp1++;

                        tmp2 += cons[tmp1 - 1];
                    }

                    //Console.WriteLine(dual[i]);
                    tw.WriteLine(dual[i]);

                }
                tw.Close();
            }
        }

        public void display_time(string insName)
        {
            using (StreamWriter tw = new StreamWriter(data.allPath.OutPutGr + insName + "timeCG.txt", true))
            {
                tw.WriteLine("{0} \t milisecond for master", master_Time);
                tw.WriteLine("{0} \t milisecond for sub exact", subMIP_time);
                tw.WriteLine("{0} \t number of column by sub exact", count_MIP);

                tw.WriteLine("**************************************************");
                tw.WriteLine("{0} \t benefit", RMP.Benefit);

                tw.Close();
            }
        }

        public bool SolveSubProblem(double[] dual, string insName)
        {

            int totalColumn = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool firstCol = false;
            for (int i = 0; i < Interns; i++)
            {
                GC.Collect();
                SubProblem sp = new SubProblem(data, dual, i, insName);
                if (sp.KeepGoing(insName))
                {
                    totalColumn++;

                    RMP.addColumn(sp.theColumn);                    
                }
            }
            if (totalColumn>0)
            {
                return true;
            }
            else
            {
                return false;
            }


        }
    }
}
