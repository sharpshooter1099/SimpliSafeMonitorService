using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliSafeMonitorService
{
    //this code was written by Alex Wardlaw, the best programmer in the world.  copyright 2015.
    public class Configurer
    {
        #region Fields

        private string fileName;

        internal bool changes;

        //internal string[] allLines;
        internal Dictionary<string, string[]> fields;

        public Dictionary<string, string[]> activeFields
        {
            get { return fields; }
        }

        #endregion Fields

        #region Public

        public Configurer(string appName)
        {
            fields = new Dictionary<string, string[]>();
            string[] allLines;

            if (appName.ToLower().Contains(@".conf"))
                fileName = appName;
            else
                fileName = string.Format(@"{0}.conf", appName);
            allLines = System.IO.File.ReadAllLines(fileName);

            char[] lineDelimiters = { '=' };
            char[] valueDelimiters = { ',' };

            foreach (string line in allLines)
            {
                if (line.Length > 0 && line[0] != '#')
                {
                    string[] split1 = line.Split(lineDelimiters,
                             StringSplitOptions.RemoveEmptyEntries);
                    string key = split1[0];

                    string[] split2 = split1[1].Split(valueDelimiters,
                             StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < split2.Length; i++)
                        split2[i] = split2[i].Trim();

                    fields.Add(key.Trim(), split2);
                }
            }

            changes = false;
        }

        public string[] RequestValues(string field)
        {
            if (fields.ContainsKey(field))
                return fields[field];
            else
                return null;

        }

        public void UpdateValues(string field, string[] values)
        {
            if (fields.ContainsKey(field))
                fields[field] = values;
            else
                fields.Add(field, values);

            changes = true;
        }

        //destructor- if changes were made write file
        ~Configurer()
        {
            if (changes)
            {
                string[] fileContents = System.IO.File.ReadAllLines(fileName);
                List<string> allLines = new List<string>();
                char[] lineDelimiters = { '=' };

                for (int i = 0; i < fileContents.Length; i++)
                {
                    string line = fileContents[i];
                    if (line.Length > 0 && line[0] != '#')
                    {
                        string tmp = line.Replace(" ", "");
                        string[] split1 = tmp.Split(lineDelimiters,
                                 StringSplitOptions.RemoveEmptyEntries);
                        string key = split1[0];
                        if (fields.ContainsKey(key))
                            line = string.Format(@"{0}={1}", key, String.Join(",", fields[key]));
                    }
                    allLines.Add(line);
                }

                System.IO.File.WriteAllLines(fileName, allLines);
            }
        }

        #endregion Public

        #region Private

        #endregion Private

        #region Internal

        #endregion Internal
    }
}
