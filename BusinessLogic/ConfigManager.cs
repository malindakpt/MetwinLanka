using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.PersisterInterface;
using DataAccessLayer;

namespace BusinessLogic
{
    public class ConfigManager : IDisposable
    {
        IConfigPersister configDb;

        const string Key_VATCHARGE = "VAT_PERCENTAGE";
        const string key_ADDRESS = "ADDRESS";
        const string Key_HIDEVATUNITPRICE = "HIDEVATUNITPRICE";
        const string Key_VATNUM = "VATNUMBER";

        const string defaultAddress = "Metwin Roofing.\nNo. 921/3, Negombo Road,\nMaththegama, Ku/Bopitiya,\nSri lanka.";

        public ConfigManager(IConfigPersister configPersister = null)
        {
            configDb = configPersister ?? new ConfigPersister();
        }

        /// <summary>
        /// Vat percentage as a decimal ( 0.15 for 15%)
        /// </summary>
        /// <returns></returns>

        public decimal VatPercentage
        {
            get
            {
                const decimal defaultVat = 15m;
                decimal vat;
                configDb.ReadSetting(Key_VATCHARGE, out vat, defaultVat);
                //update to new format to store as percentage
                if(vat < 0.25m)
                {
                    vat *= 100;
                    configDb.WriteSetting(Key_VATCHARGE, vat);
                }
                return vat/100;
            }
            set
            {
                //save as percentage
                configDb.WriteSetting(Key_VATCHARGE, value * 100);
            }
        }

        public string Address
        {
            get
            {
                string addr;
                configDb.ReadSetting(key_ADDRESS, out addr, defaultAddress);
                return addr;
            }
            set
            {
                configDb.WriteSetting(key_ADDRESS, value);
            }
        }

        public bool HideUnitPriceInVatInvoice
        {
            get
            {
                int hide;
                configDb.ReadSetting(Key_HIDEVATUNITPRICE, out hide, 0);
                return hide != 0;
            }
            set
            {
                configDb.WriteSetting(Key_HIDEVATUNITPRICE, value ? 1 : 0);
            }
        }

        public string VatNumber
        {
            get
            {
                string vat;
                configDb.ReadSetting(Key_VATNUM, out vat, "");
                return vat;
            }
            set
            {
                configDb.WriteSetting(Key_VATNUM, value);
            }
        }

        public void Dispose()
        {
            configDb.Dispose();
        }
    }
}
