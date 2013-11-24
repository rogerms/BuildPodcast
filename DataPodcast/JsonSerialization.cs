using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace DataPodcast
{
    public class JsonSerialization
    {
        public static string WriteFromObject(ObservableCollection<Podcast>  podcastCollection)
        {

            DataContractJsonSerializer fg = new DataContractJsonSerializer(typeof(ObservableCollection<Podcast>));
            MemoryStream myStrem = new MemoryStream();
            fg.WriteObject(myStrem, podcastCollection);
            byte[] json = myStrem.ToArray();

            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        // Deserialize a JSON stream to a User object.
        public static ObservableCollection<Podcast> ReadToObject(string json)
        {
            ObservableCollection<Podcast> deserializedObj = new ObservableCollection<Podcast>();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedObj.GetType());
            deserializedObj = ser.ReadObject(ms) as ObservableCollection<Podcast>;
            return deserializedObj;
        }
    }
}
