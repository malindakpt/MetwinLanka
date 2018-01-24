using BusinessObjects;
using BusinessObjects.Categories;
using DataAccessLayer.PersisterInterface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ProductPersister : IProductPersister
    {
        AppDBContext db = new AppDBContext();
        public IEnumerable<ProductProfile> GetProductProfiles(bool includeDisabled = false)
        {
            var results = db.ItemProfiles.AsNoTracking().OfType<ProductProfile>();
            if (!includeDisabled)
            {
                results = results.Where(i => i.IsEnabled);
            }
            return results.ToList();
        }

        public IEnumerable<AccountProfile> GetAccountProfiles(bool includeDisabled = false)
        {
            var results = db.ItemProfiles.AsNoTracking().OfType<AccountProfile>();
            if (!includeDisabled)
            {
                results = results.Where(i => i.IsEnabled);
            }
            return results.ToList();
        }

        public IEnumerable<RawMaterialProfile> GetRawMaterialProfiles(bool includeDisabled = false)
        {
            var results= db.ItemProfiles.AsNoTracking().OfType<RawMaterialProfile>();
            if(!includeDisabled)
            {
                results = results.Where(i => i.IsEnabled);
            }
            return results.ToList();
        }

        public void Dispose()
        {
            db.Dispose();
        }


        public void AddProfile(ProductProfile profile)
        {
            var rmIds = profile.RawMaterialProfiles.Select(p => p.Id).Distinct();
           // profile.RawMaterialProfiles = new List<RawMaterialProfile>();
            

            var rms = rmIds.Select(id => GetProfile<RawMaterialProfile>(id, false));
            profile.RawMaterialProfiles = rms.ToList();
            db.ItemProfiles.Add(profile);
           // db.Entry(profile).State = EntityState.Modified;            
            db.SaveChanges();
        }
        public void AddProfile(RawMaterialProfile profile)
        {

            db.ItemProfiles.Add(profile);
            db.SaveChanges();
        }

        public void AddProfile(AccountProfile profile)
        {

            db.ItemProfiles.Add(profile);
            db.SaveChanges();
        }


        public ProductProfile GetProductProfile(int id)
        {
            var prof = GetProfile<ProductProfile>(id,true);
            
            return prof;
        }

        public void UpdateProfile<T>(T profile) where T: ItemProfile
        {
            db.Entry<T>(profile).State =EntityState.Modified;
            db.SaveChanges();
        }


        public RawMaterialProfile GetRawMaterialProfile(int id)
        {
            var prof = GetProfile<RawMaterialProfile>(id,true);
            return prof;
        }

        public AccountProfile GetAccountProfile(int id)
        {
            var prof = GetProfile<AccountProfile>(id, true);
            return prof;
        }


        public void UpdateProfile(ProductProfile updated)
        {
            var newRM = new HashSet<int>(updated.RawMaterialProfiles.Select(rm => rm.Id));
            db.Entry(updated).State = EntityState.Modified;
            db.SaveChanges();
            db.Entry(updated).State = EntityState.Detached;

            var ori =  GetProfile<ProductProfile>(updated.Id);

            var oriRM = new HashSet<int>(ori.RawMaterialProfiles.Select(rm => rm.Id));
            

            var added = newRM.Except(oriRM);
            var removed = oriRM.Except(newRM);

            foreach (var id in added)
            {
                var rm = GetProfile<RawMaterialProfile>(id);
                ori.RawMaterialProfiles.Add(rm);
            }

            foreach (var id in removed)
            {
                var rm = GetProfile<RawMaterialProfile>(id);
                ori.RawMaterialProfiles.Remove(rm);
            }

            db.SaveChanges();
            db.Entry(ori).State = EntityState.Detached;


        }

        public void UpdateProfile(RawMaterialProfile profile)
        {
            db.Entry(profile).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void UpdateProfile(AccountProfile profile)
        {
            db.Entry(profile).State = EntityState.Modified;
            db.SaveChanges();
        }

        private T GetProfile<T>(int id,bool detached = false) where T: ItemProfile
        {
            IQueryable<ItemProfile> source = db.ItemProfiles;
            if(detached)
            {
                source = source.AsNoTracking();
            }
            return source.OfType<T>().Where(p => p.Id == id).FirstOrDefault();
        }
    }

}
