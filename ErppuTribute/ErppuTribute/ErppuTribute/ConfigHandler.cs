/****************************************************
 * Class: ConfigHandler
 * Description: Reads and writes the game config
 * Author(s): Jonah Ahvonen, Matti Jokitulppo
 * Date: April 7, 2014
****************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ErppuTribute
{
    class ConfigHandler
    {
        #region Fields
        private string configPath;
        public Dictionary<string, object> ConfigBundle { get; set; }

        private System.Xml.Serialization.XmlSerializer serializer;
        private System.IO.StreamWriter streamWriter;
        private System.IO.StreamReader streamReader;

        private bool pathSuccess = false;
        #endregion
        #region Constructor
        public ConfigHandler()
        {
            try
            {
                configPath = ConfigurationManager.AppSettings["cfgpath"]; //Yritetään lukea config-tiedoston polku appconfigista
                pathSuccess = true;
            }
            catch(ConfigurationErrorsException cee)
            {
                Console.WriteLine(cee.BareMessage);
                pathSuccess = false;
            }

            ConfigBundle = new Dictionary<string, object>();

            serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<KeyValuePair<string, object>>));
        }
        #endregion
        #region Methods
        public bool WriteConfig()
        {
            if (pathSuccess)
            {
                streamWriter = new System.IO.StreamWriter(configPath);
            }
            else
            {
                streamWriter = new System.IO.StreamWriter("eerotributedefaultconfig.xml");
            }
            
            if (ConfigBundle.Count > 0)
            {
                try
                {
                    //Muunnetaan dictionary listaksi avain-arvopareja, serialisoidaan se ja kirjoitetaan xml-tiedostoon
                    serializer.Serialize(streamWriter, dictionaryToList(ConfigBundle));
                    streamWriter.Close();
                    return true;
                }
                catch (Exception AnyException)
                {
                    MessageBox.Show(AnyException.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void ReadConfig()
        {
            try
            {
                if (pathSuccess)
                {
                    streamReader = new System.IO.StreamReader(configPath);
                }
                else
                {
                    streamReader = new System.IO.StreamReader("eerotributedefaultconfig.xml");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Couldn't find the config file.");
            }
            
            try
            {
                //Luetaan ja deserialisoidaan xml-tiedostosta lista avain-arvopareja, muunnetaan se dictionaryksi ja tallennetaan propertyyn
                ConfigBundle = listToDictionary((List<KeyValuePair<string, object>>)serializer.Deserialize(streamReader));
                streamReader.Close();
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("Couldn't find the config file.");
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Couldn't find the config file.");
            }
            catch (Exception g)
            {
                MessageBox.Show(g.ToString());
            }
        }
       
        //Converttaa lista avain-arvopareja dictionaryksi (Dictionaryä mukavampi käyttää)
        private Dictionary<string, object> listToDictionary(List<KeyValuePair<string, object>> list)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            foreach(KeyValuePair<string, object> pair in list)
            {
                dictionary.Add(pair.Key, pair.Value);
                Console.WriteLine(pair.Key + " | " + pair.Value);
            }

            return dictionary;
        }

        //Converttaa dictionaryn listaksi avain-arvopareja (Dictionarya ei voi serialisoida)
        private List<KeyValuePair<string, object>> dictionaryToList(Dictionary<string, object> dictionary)
        {
            List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();

            KeyValuePair<string, object> kvp; //Käytetään "overridattuja" avain-arvopareja, jotka voi serialisoida

            for(int i = 0; i < dictionary.Count; i++)
            {
                kvp = new KeyValuePair<string, object>();
                kvp.Key = dictionary.ElementAt(i).Key;
                kvp.Value = dictionary.ElementAt(i).Value;

                list.Add(kvp);    
            }

            return list;
        }
        #endregion
    }
}
