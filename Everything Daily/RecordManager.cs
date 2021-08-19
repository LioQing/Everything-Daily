using Microsoft.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

namespace Everything_Daily
{
    public class RecordType
    {
        public string Name { get; set; }
        public Color Color { get; set; }

        public RecordType(string Name, Color Color)
        {
            this.Name = Name;
            this.Color = Color;
        }
    }

    public class RecordManager
    {
        public IDictionary<string, RecordType> RecordTypes { get; set; } = new Dictionary<string, RecordType>();
        public IList<Record> Records { get; set; } = new List<Record>();

        public string GetRecordName(Record record)
        {
            return RecordTypes[record.Id].Name;
        }

        public Color GetRecordColor(Record record)
        {
            return RecordTypes[record.Id].Color;
        }

        private async Task SaveStr(string str)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync("save.json");

            StorageFile file;
            if (item is null)
            {
                file = await ApplicationData.Current.LocalFolder.CreateFileAsync("save.json",
                    CreationCollisionOption.ReplaceExisting);
            }
            else
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync("save.json");
            }
            await FileIO.WriteTextAsync(file, str);
        }

        public void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(this, options);

            var task = Task.Run(() => SaveStr(jsonString));
            task.Wait();
        }

        private async Task<string> ReadFromSave()
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync("save.json");
            return await FileIO.ReadTextAsync(file);
        }

        public void Load()
        {
            string jsonString;
            RecordManager tmpThis;
            try
            {
                var task = Task.Run(ReadFromSave);
                jsonString = task.Result;
                tmpThis = JsonSerializer.Deserialize<RecordManager>(jsonString);
            }
            catch (Exception)
            {
                Save();
                var task = Task.Run(ReadFromSave);
                jsonString = task.Result;
                tmpThis = JsonSerializer.Deserialize<RecordManager>(jsonString);
            }
            RecordTypes = tmpThis.RecordTypes;
            Records = tmpThis.Records;
        }

        public void Load(string jsonString)
        {
            RecordManager tmpThis;
            tmpThis = JsonSerializer.Deserialize<RecordManager>(jsonString);
            RecordTypes = tmpThis.RecordTypes;
            Records = tmpThis.Records;
        }
    }
}
