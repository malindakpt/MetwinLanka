using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.PersisterInterface
{
    public interface IConfigPersister :IDisposable
    {
        /// <summary>
        /// Reads a setting
        /// </summary>
        /// <param name="key">setting name</param>
        /// <param name="value">out variable to store</param>
        /// <param name="defaultValue">value if setting doesn't exist</param>
        /// <returns>True if value setting exists, false otherwise</returns>
        bool ReadSetting(string key, out int value, int defaultValue);

        /// <summary>
        /// Reads a setting
        /// </summary>
        /// <param name="key">setting name</param>
        /// <param name="value">out variable to store</param>
        /// <param name="defaultValue">value if setting doesn't exist</param>
        /// <returns>True if value setting exists, false otherwise</returns>
        bool ReadSetting(string key, out decimal value, decimal defaultValue);

        /// <summary>
        /// Reads a setting
        /// </summary>
        /// <param name="key">setting name</param>
        /// <param name="value">out variable to store</param>
        /// <param name="defaultValue">value if setting doesn't exist</param>
        /// <returns>True if value setting exists, false otherwise</returns>
        bool ReadSetting(string key, out string value, string defaultValue);

        void WriteSetting(string key, int value);
        void WriteSetting(string key, decimal value);
        void WriteSetting(string key, string value);
    }
}
