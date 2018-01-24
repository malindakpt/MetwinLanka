using BusinessObjects;
using BusinessObjects.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.PersisterInterface
{
    public enum ProfileTypes
    {
        Product, RawMaterial, Accounting
    }

    public interface IProductPersister : IDisposable
    {
        IEnumerable<ProductProfile> GetProductProfiles(bool includeDisabled = false);
        IEnumerable<AccountProfile> GetAccountProfiles(bool includeDisabled = false);
        IEnumerable<RawMaterialProfile> GetRawMaterialProfiles(bool includeDisabled = false);


        void AddProfile(RawMaterialProfile profile);
        void AddProfile(ProductProfile profile);
        void AddProfile(AccountProfile profile);

        ProductProfile GetProductProfile(int id);
        RawMaterialProfile GetRawMaterialProfile(int id);
        AccountProfile GetAccountProfile(int id);

        void UpdateProfile(ProductProfile profile);
        void UpdateProfile(RawMaterialProfile profile);
        void UpdateProfile(AccountProfile profile);
    }
}
