using BusinessObjects.Categories;
using DataAccessLayer;
using DataAccessLayer.PersisterInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class ProfileManager : IDisposable
    {
        IProductPersister db;

        public ProfileManager(IProductPersister persister = null)
        {
            db = persister ?? new ProductPersister();
        }

        public IEnumerable<ProductProfile> GetProductProfiles(bool includeInactive = false)
        {
            return db.GetProductProfiles(includeInactive);
        }

        public IEnumerable<AccountProfile> GetAccountProfiles(bool includeInactive = false)
        {
            return db.GetAccountProfiles(includeInactive);
        }


        public ProductProfile GetProductProfile(int id)
        {
            return db.GetProductProfile(id);
        }

        public void AddProductProfile(ProductProfile prof)
        {
            db.AddProfile(prof);
        }

        public void AddAccountProfile(AccountProfile prof)
        {
            db.AddProfile(prof);
        }

        public void UpdateProductProfile(ProductProfile prof)
        {
            db.UpdateProfile(prof);
        }


        public IEnumerable<RawMaterialProfile> GetRawMaterialProfiles(bool includeInactive = false)
        {
            return db.GetRawMaterialProfiles(includeInactive);
        }

        public RawMaterialProfile GetRawMaterialProfile(int id)
        {
            return db.GetRawMaterialProfile(id);
        }

        public void AddRawMaterialProfile(RawMaterialProfile prof)
        {
            db.AddProfile(prof);
        }

        public void UpdateRawMaterialProfile(RawMaterialProfile prof)
        {
            db.UpdateProfile(prof);
        }



        public void Dispose()
        {
            db.Dispose();
        }
    }
}
