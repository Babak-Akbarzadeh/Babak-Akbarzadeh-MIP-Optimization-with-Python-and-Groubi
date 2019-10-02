﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BranchAndPriceAlgorithm
{
    class Column
    {

        public void WriteXML(string Path)
        {
            Path = Path + "Info.xml";
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(Column));
            System.IO.FileStream file = System.IO.File.Create(Path);
            writer.Serialize(file, this);
            file.Close();
        }

        public Column ReadXML(string Path)
        {
            // Now we can read the serialized book ...  
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(Column));

            System.IO.StreamReader file = new System.IO.StreamReader(Path);
            Column tmp = (Column)reader.Deserialize(file);
            file.Close();
            return tmp;
        }
    }
}
