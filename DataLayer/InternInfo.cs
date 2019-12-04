﻿using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace DataLayer
{
	public struct DesirePos
	{
		public int theD;
		public int theH;
		public double Desire;
		public double AbsDesire;
		
		DesirePos(int theD, int theH, double Desire, double AbsDesire)
		{
			this.theD = theD;
			this.theH = theH;
			this.Desire = Desire;
			this.AbsDesire = AbsDesire;
		}
		public void Copy(DesirePos copyable)
		{
			theD = copyable.theD;
			theH = copyable.theH;
			Desire = copyable.Desire;
			AbsDesire = copyable.AbsDesire;
		}
	}
	public class InternInfo
	{
		public string Name;
		public int SQLID;
		public int[] Prf_d;
		public int[] Prf_h;
		public bool[] Ave_t;
		public bool[][] Abi_dh;
		public int wieght_d;
		public int wieght_h;
		public int wieght_w;
		public int wieght_ch;
        public int wieght_cns;
        public int ProgramID;
		public bool[] TransferredTo_r;
		public bool[][][] Fulfilled_dhp;
		public bool[][] OverSea_dt;
		public bool[] FHRequirment_d;
		public bool overseaReq;
		public int[] SortedPrfD_pos;
		public int[] SortedPrfH_pos;
		public int[][] AllDes_dh;
		//public int MaxDesireHos;
		//public int MaxDesireDis;
		//public double MaxDesir;
		public ArrayList sortedPrf;
		public double MaxPrf;
		public bool[][] DisciplineList_dg;
		public int[] ShouldattendInGr_g;
		public bool isProspective;
		public int K_AllDiscipline;
        public int K_CountDiscipline;
		public double[] takingDiscPercentage;
		public double MaxPrfDiscipline;
		public double AveDur;
        public bool[] twinIntern;

        public int[] sortedTimeOrder;

		public double[] requieredTimeForRemianed;
		public InternInfo(int Hospitals, int Disciplines, int TimePeriods, int DisciplineGr, int TrainingPr, int Region)
		{
			Name = "";
			SQLID = 0;
			new ArrayInitializer().CreateArray(ref Prf_d, Disciplines, 0);
			new ArrayInitializer().CreateArray(ref FHRequirment_d, Disciplines, false);
			new ArrayInitializer().CreateArray(ref TransferredTo_r, Region, false);
			new ArrayInitializer().CreateArray(ref ShouldattendInGr_g, DisciplineGr, 0);
			new ArrayInitializer().CreateArray(ref Prf_h, Hospitals, 0);
			new ArrayInitializer().CreateArray(ref SortedPrfD_pos, Disciplines, 0);
			new ArrayInitializer().CreateArray(ref SortedPrfH_pos, Hospitals, 0);
			new ArrayInitializer().CreateArray(ref Ave_t, TimePeriods, true);
			new ArrayInitializer().CreateArray(ref Abi_dh, Disciplines, Hospitals, true);
			new ArrayInitializer().CreateArray(ref Fulfilled_dhp, Disciplines, Hospitals, TrainingPr, false);
			new ArrayInitializer().CreateArray(ref DisciplineList_dg, Disciplines, DisciplineGr, false);
			new ArrayInitializer().CreateArray(ref OverSea_dt, Disciplines, TimePeriods, false);
			new ArrayInitializer().CreateArray(ref AllDes_dh,Disciplines, Hospitals, -1);
			new ArrayInitializer().CreateArray(ref takingDiscPercentage, Disciplines, 0);
            new ArrayInitializer().CreateArray(ref sortedTimeOrder, Disciplines, -1);
            wieght_d = 0;
			wieght_h = 0;
			wieght_w = 0;
			wieght_ch = 0;
            wieght_cns = 0;
            ProgramID = -1;
			isProspective = true;
			K_AllDiscipline = 0;
			overseaReq = false;
			//MaxDesir = 0;
			//MaxDesireDis = 0;
			//MaxDesireHos = 0;
		}

		public void sortPrf(int Hospitals, int Disciplines, int DisciplineGr , int HospitalWard, AllData allData)
		{
			sortedPrf = new ArrayList();
			MaxPrfDiscipline = 0;
			K_AllDiscipline = 0;
			for (int g = 0; g < DisciplineGr; g++)
			{
				K_AllDiscipline += ShouldattendInGr_g[g];
			}
			for (int d = 0; d < Disciplines ; d++)
			{
                bool notInvoleved = true;
                for (int g = 0; g < allData.General.DisciplineGr; g++)
                {
                    if (DisciplineList_dg[d][g])
                    {
                        notInvoleved = false;
                        break;
                    }
                }
                if (notInvoleved)
                {
                    continue;
                }
				if (MaxPrfDiscipline < Prf_d[d])
				{
					MaxPrfDiscipline = Prf_d[d];
				}
				for (int h = 0; h < Hospitals; h++)
				{					
					double MMaxDesire = -1;
					double AbsMaxDesire = -1;
					for (int w = 0; w < HospitalWard ; w++)
					{
						if (allData.Hospital[h].Hospital_dw[d][w])
						{
							bool overS = false;
							for (int t = 0; t < allData.General.TimePriods; t++)
							{
								if (OverSea_dt[d][t])
								{
									overS = true;
									break;
								}
							}
							AbsMaxDesire = (double)(Prf_d[d] * wieght_d
										+ Prf_h[h] * wieght_h
										+ allData.TrainingPr[ProgramID].weight_p * allData.TrainingPr[ProgramID].Prf_d[d]) / allData.Discipline[d].CourseCredit_p[ProgramID];
                            int Maxcns = 0;
                            for (int dd = 0; dd < Disciplines; dd++)
                            {
                                if (allData.TrainingPr[ProgramID].cns_dD[d][dd] > Maxcns)
                                {
                                    Maxcns = allData.TrainingPr[ProgramID].cns_dD[d][dd];
                                }
                            }
                            AbsMaxDesire += wieght_cns * Maxcns;
                            if (overS)
							{
                                AbsMaxDesire -= (double)(Prf_h[h] * wieght_h) / allData.Discipline[d].CourseCredit_p[ProgramID];
							}
							MMaxDesire = AbsMaxDesire * takingDiscPercentage[d] ;
							
						}
					}
					if (AbsMaxDesire != -1)
					{
						int counter = 0;
						foreach (DesirePos item in sortedPrf)
						{
							if (item.AbsDesire > AbsMaxDesire)
							{
								counter++;
							}
							else
							{
								break;
							}
						}
                        for (int cc = 0; cc < allData.Discipline[d].CourseCredit_p[ProgramID]; cc++)
                        {
                            sortedPrf.Insert(counter, new DesirePos()
                            {
                                theD = d,
                                theH = h,
                                Desire = MMaxDesire,
                                AbsDesire = AbsMaxDesire
                            });
                        }
						
					}
					
				}
			}

			MaxPrf = 0;
			for (int c = 0; c < K_AllDiscipline; c++)
			{
				MaxPrf += ((DesirePos)sortedPrf[c]).Desire * allData.TrainingPr[ProgramID].CoeffObj_SumDesi;
			}
	
		}
		public void setKAllDiscipline(AllData allData)
		{
            K_AllDiscipline = 0;

            for (int g = 0; g < allData.General.DisciplineGr; g++)
			{
				K_AllDiscipline += this.ShouldattendInGr_g[g];

			}
            sortTime(allData);
		}

		public void setAveInfo(AllData allData)
		{
			int counter = 0;
			AveDur = 0;
			ArrayList sequence = new ArrayList();
			for (int d = 0; d < allData.General.Disciplines; d++)
			{
				for (int g = 0; g < allData.General.DisciplineGr; g++)
				{
					if (DisciplineList_dg[d][g])
					{
                        for (int cc = 0; cc < allData.Discipline[d].CourseCredit_p[ProgramID]; cc++)
                        {
                            double xx = (double)allData.Discipline[d].Duration_p[ProgramID] / allData.Discipline[d].CourseCredit_p[ProgramID];
                            sequence.Add(xx);
                        }
						break;
					}
				}
			}
			double[] seq = new double[sequence.Count];
			for (int s = 0; s < sequence.Count; s++)
			{
				// to sort increasing 
				seq[s] = -((double)sequence[s]);
			}

			AveDur = Percentile(seq, 0.5);
		}

		public double Percentile(double[] sequence, double excelPercentile)
		{
			Array.Sort(sequence);

			//first set this data
			requieredTimeForRemianed = new double[K_AllDiscipline];
			
			for (int i = 0; i < K_AllDiscipline; i++)
			{
				for (int j = i; j < K_AllDiscipline; j++)
				{
					requieredTimeForRemianed[i] += (-sequence[j]);
				}
			}
			Array.Sort(requieredTimeForRemianed);
			double sum = 0;
			int N = sequence.Length;
			double n = (N - 1) * excelPercentile + 1;
			// Another method: double n = (N + 1) * excelPercentile;
			for (int i = 0; i < n && i < sequence.Count(); i++)
			{
				sum += sequence[i];
			}
			if (n > 1)
			{
				n = (int)n;
			}
			else
			{
				n = 1;
			}
			return -sum/n;
		}

		public void setThePercetage(AllData allData)
		{

			int totalDiscList = 0;
            
			for (int d = 0; d < allData.General.Disciplines; d++)
			{
				for (int g = 0; g < allData.General.DisciplineGr; g++)
				{
					if (DisciplineList_dg[d][g])
					{
						totalDiscList += allData.Discipline[d].CourseCredit_p[ProgramID] ;
						break; // no mutual discipline
					}
				}
			}

			for (int d = 0; d < allData.General.Disciplines;)
			{
				takingDiscPercentage[d] = (double)K_AllDiscipline / totalDiscList;
                d += allData.Discipline[d].CourseCredit_p[ProgramID]; 
			}

			for (int g = 0; g < allData.General.DisciplineGr; g++)
			{
				int totalGr = 0;
				for (int d = 0; d < allData.General.Disciplines; d++)
				{
					if (DisciplineList_dg[d][g])
					{
						totalGr += allData.Discipline[d].CourseCredit_p[ProgramID];
                    }
				}
				for (int d = 0; d < allData.General.Disciplines; d++)
				{
					if (DisciplineList_dg[d][g])
					{
						takingDiscPercentage[d] = (double)ShouldattendInGr_g[g] / totalGr;
					}
				}
			}

			for (int d = 0; d < allData.General.Disciplines; d++)
			{
				if (takingDiscPercentage[d] >= 0.95 && allData.Discipline[d].requiresSkill_p[ProgramID])
				{
					for (int dd = 0; dd < allData.General.Disciplines; dd++)
					{
						if (allData.Discipline[d].Skill4D_dp[dd][ProgramID])
						{
							takingDiscPercentage[dd] = 1;
						}
					}
				}
				for (int t = 0; t < allData.General.TimePriods; t++)
				{
					if (OverSea_dt[d][t])
					{
						takingDiscPercentage[d] = 1;
						for (int dd = 0; dd < allData.General.Disciplines; dd++)
						{
							if (FHRequirment_d[dd])
							{
								takingDiscPercentage[dd] = 1;
							}
						}
					}
				}
			}
		}

		public void SetSimplifiedScheduleForMaxPrf(AllData allData)
		{
            int totalAssignment = 0;
			int tmpKAll = K_AllDiscipline;
			int[] tmpK_g = new int[allData.General.DisciplineGr];
            bool[] disStatus = new bool[allData.General.Disciplines];
            for (int g = 0; g < allData.General.DisciplineGr; g++)
			{
				tmpK_g[g] = ShouldattendInGr_g[g];
			}
			MaxPrf = 0;
			// first assign foregn hospital and related requirements 
			for (int d = 0; d < allData.General.Disciplines; d++)
			{
				bool overS = false;
                disStatus[d] = false;
				for (int t = 0; t < allData.General.TimePriods; t++)
				{
					if (OverSea_dt[d][t])
					{
						overS = true;
						break;
					}
				}
				if (overS)
				{
					// find the best place for d
					int c = -1;
					foreach (DesirePos item in sortedPrf)
					{
						c++;
						if (item.theD == d)
						{							
							break;
						}
					}
					if (c >= 0)
					{
						tmpKAll -= allData.Discipline[d].CourseCredit_p[ProgramID];
						MaxPrf += allData.Discipline[d].CourseCredit_p[ProgramID] * ((DesirePos)sortedPrf[c]).AbsDesire * allData.TrainingPr[ProgramID].CoeffObj_SumDesi;
                        disStatus[d] = true;
                        removeDisciplineFromList(allData, d);
                        totalAssignment++;
					}
					// now the requiremnets 
					// find the group
					for (int g = 0; g < allData.General.DisciplineGr; g++)
					{
						if (DisciplineList_dg[d][g])
						{
							tmpK_g[g]-= allData.Discipline[d].CourseCredit_p[ProgramID];
							break;
						}
						
					}

					for (int dd = 0; dd < allData.General.Disciplines; dd++)
					{
						if (FHRequirment_d[dd])
						{
							c = -1;
							foreach (DesirePos item in sortedPrf)
							{
								c++;
								if (item.theD == d)
								{
									break;
								}
							}
							if (c >= 0)
							{
								tmpKAll-= allData.Discipline[d].CourseCredit_p[ProgramID];
								MaxPrf += allData.Discipline[d].CourseCredit_p[ProgramID] * ((DesirePos)sortedPrf[c]).AbsDesire * allData.TrainingPr[ProgramID].CoeffObj_SumDesi;
                                disStatus[d] = true;
                                removeDisciplineFromList(allData, d);
                                totalAssignment++;
                                // find the group
                                for (int g = 0; g < allData.General.DisciplineGr; g++)
                                {
                                    if (DisciplineList_dg[dd][g])
                                    {
                                        tmpK_g[g] -= allData.Discipline[dd].CourseCredit_p[ProgramID];
                                        break;
                                    }

                                }
                            }
						}
					}
				}
			}

			// the rest of schedule 
			int IndeXarry = 0;
			// with this while it is not always a max prf
			while (tmpKAll > 0 && IndeXarry < sortedPrf.Count)
			{
				DesirePos tmp = (DesirePos)sortedPrf[IndeXarry];
				int GrIndex = -1;
				for (int g = 0; g < allData.General.DisciplineGr; g++)
				{
					if (DisciplineList_dg[tmp.theD][g] && tmpK_g[g] > 0)
					{
						GrIndex = g;
						break;
					}
				}
				if (GrIndex >= 0)
				{
					tmpKAll-= allData.Discipline[tmp.theD].CourseCredit_p[ProgramID];
					tmpK_g[GrIndex]-= allData.Discipline[tmp.theD].CourseCredit_p[ProgramID];
					MaxPrf += allData.Discipline[tmp.theD].CourseCredit_p[ProgramID] * tmp.AbsDesire;
                    disStatus[tmp.theD] = true;
                    removeDisciplineFromList(allData, tmp.theD);
                    totalAssignment++;
                    IndeXarry = 0;
                }
				else
				{
					IndeXarry++;
				}
				
			}

			// if we could not manage to consider the group K
			IndeXarry = 0;
			while (tmpKAll > 0 && IndeXarry < sortedPrf.Count)
			{
				DesirePos tmp = (DesirePos)sortedPrf[IndeXarry];
				int GrIndex = -1;
				for (int g = 0; g < allData.General.DisciplineGr; g++)
				{
					if (DisciplineList_dg[tmp.theD][g])
					{
						GrIndex = g;
						break;
					}
				}
				if (GrIndex >= 0)
				{
					tmpKAll -= allData.Discipline[tmp.theD].CourseCredit_p[ProgramID];
					MaxPrf += allData.Discipline[tmp.theD].CourseCredit_p[ProgramID] * tmp.AbsDesire * allData.TrainingPr[ProgramID].CoeffObj_SumDesi;
                    disStatus[tmp.theD] = true;
                    removeDisciplineFromList(allData, tmp.theD);
                    totalAssignment++;
                    IndeXarry = 0;

                }
				else
				{
					IndeXarry++;
				}

			}

			if (allData.TrainingPr[ProgramID].DiscChangeInOneHosp == 1)
			{
				MaxPrf -= wieght_ch * (totalAssignment - 1) * allData.TrainingPr[ProgramID].CoeffObj_SumDesi;
			}


            // consecutive
		}

		public void removeDisciplineFromList(AllData allData, int theD)
		{
			int c = 0;
			while (c < sortedPrf.Count)
			{
                // same discipline or AKA
				if (((DesirePos)sortedPrf[c]).theD == theD || allData.TrainingPr[ProgramID].AKA_dD[((DesirePos)sortedPrf[c]).theD][theD])
				{
					sortedPrf.RemoveAt(c);
					c--;
                }
				c++;
			}
		}

        public void setTwinInterns(AllData allData) {
            new ArrayInitializer().CreateArray(ref twinIntern, allData.General.Interns , false);
            for (int i = 0; i < allData.General.Interns; i++)
            {
                if (allData.Intern[i].K_AllDiscipline <= K_AllDiscipline && allData.Intern[i].ProgramID == ProgramID)
                {
                    bool isTwin = true;
                    for (int g = 0; g < allData.General.DisciplineGr; g++)
                    {
                        if (allData.Intern[i].ShouldattendInGr_g[g] > ShouldattendInGr_g[g])
                        {
                            isTwin = false;
                            break;
                        }
                    }
                    if (isTwin)
                    {
                        twinIntern[i] = true;
                    }
                }
            }

        }

        public void sortTime(AllData allData) 
        {
            for (int d = 0; d < allData.General.Disciplines; d++)
            {
                sortedTimeOrder[d] = d;
            }

            for (int d = 0; d < allData.General.Disciplines; d++)
            {
                for (int dd = d + 1; dd < allData.General.Disciplines; dd++)
                {
                    int dInd = sortedTimeOrder[d];
                    int ddInd = sortedTimeOrder[dd];
                    if (allData.Discipline[dInd].Duration_p[ProgramID] > allData.Discipline[ddInd].Duration_p[ProgramID])
                    {
                        sortedTimeOrder[d] = ddInd;
                        sortedTimeOrder[dd] = dInd;
                    }
                }
            }

        }

        public int requiredTime(bool[] involvedDisc, int[] remainedK_g,int remaindedKall, AllData allData) 
        {
            int result = 0;
            int[] K_g = new int[allData.General.DisciplineGr];
            bool[] disStatus = new bool[allData.General.Disciplines];
            int Kall = remaindedKall;
            for (int g = 0; g < allData.General.DisciplineGr; g++)
            {
                K_g[g] = remainedK_g[g];
            }
            for (int d = 0; d < allData.General.Disciplines; d++)
            {
                disStatus[d] = involvedDisc[d];
            }
            for (int d = 0; d < allData.General.Disciplines && remaindedKall > 0; d++)
            {
                int dIndex = sortedTimeOrder[d];
                if (disStatus[dIndex])
                {
                    continue;
                }
                bool flg = false;
                int gInd = -1;
                for (int g = 0; g < allData.General.DisciplineGr; g++)
                {
                    if (DisciplineList_dg[dIndex][g] && K_g[g] > 0)
                    {
                        flg = true;
                        gInd = g;
                    }
                }
                if (flg)
                {
                    result += allData.Discipline[dIndex].Duration_p[ProgramID];
                    K_g[gInd]--;
                    disStatus[dIndex] = true;
                    remaindedKall--;
                }
            }


            return result;
        }
	}
}
