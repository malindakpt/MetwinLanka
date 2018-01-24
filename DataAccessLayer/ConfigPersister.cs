using DataAccessLayer.PersisterInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ConfigPersister : IConfigPersister
    {
        AppDBContext db = new AppDBContext();

        public void Dispose()
        {
            db.Dispose();
        }

        public bool ReadSetting(string key, out string value, string defaultValue)
        {
            var setting = db.AppSettings.FirstOrDefault(cfg => cfg.Key == key);
            if(setting != null )
            {
                value = setting.StringValue ?? defaultValue;
                return true;
            }
            else
            {
                value = defaultValue;
                return false;
            }
        }

        public bool ReadSetting(string key, out decimal value, decimal defaultValue)
        {
            var setting = db.AppSettings.FirstOrDefault(cfg => cfg.Key == key);
            if (setting != null)
            {
                value = setting.DecimalValue ?? defaultValue;
                return true;
            }
            else
            {
                value = defaultValue;
                return false;
            }
        }

        public bool ReadSetting(string key, out int value, int defaultValue)
        {
            var setting = db.AppSettings.FirstOrDefault(cfg => cfg.Key == key);
            if (setting != null)
            {
                value = setting.IntValue ?? defaultValue;
                return true;
            }
            else
            {
                value = defaultValue;
                return false;
            }
        }

        public void WriteSetting(string key, string value)
        {
            var setting = db.AppSettings.FirstOrDefault(cfg => cfg.Key == key);
            if(setting != null)
            {
                setting.StringValue = value;
            }
            else
            {
                setting = new Util.AppSetting { Key = key, StringValue = value };
                db.AppSettings.Add(setting);
            }
            db.SaveChanges();
        }

        public void WriteSetting(string key, decimal value)
        {
            var setting = db.AppSettings.FirstOrDefault(cfg => cfg.Key == key);
            if (setting != null)
            {
                setting.DecimalValue = value;
            }
            else
            {
                setting = new Util.AppSetting { Key = key, DecimalValue = value };
                db.AppSettings.Add(setting);
            }
            db.SaveChanges();
        }

        public void WriteSetting(string key, int value)
        {
            var setting = db.AppSettings.FirstOrDefault(cfg => cfg.Key == key);
            if (setting != null)
            {
                setting.IntValue = value;
            }
            else
            {
                setting = new Util.AppSetting { Key = key, IntValue = value };
                db.AppSettings.Add(setting);
            }
            db.SaveChanges();
        }
    }
}
