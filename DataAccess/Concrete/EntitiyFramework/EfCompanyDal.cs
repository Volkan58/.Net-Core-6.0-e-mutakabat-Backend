using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntitiyFramework.Context;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntitiyFramework
{
    public class EfCompanyDal : EfEntityBase<Company, ContextDb>, ICompanyDal
    {
        public UserCompany GetCompany(int userId)
        {
            using (var context=new ContextDb())
            {
                var result = context.UserCompanies.Where(m => m.UserId == userId).FirstOrDefault();
                return result;
            }
        }

        public void UserCompanyAdd(int userId, int companyId)
        {
            using (var context=new ContextDb())
            {
                UserCompany userCompany = new UserCompany()
                {
                    UserId = userId,
                    CompanyId=companyId,
                    AddedAt = DateTime.Now,
                    IsActive = true,

                };
                context.UserCompanies.Add(userCompany);
                context.SaveChanges();

            }
        }
    }
}
