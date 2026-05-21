using SljemeTimeAttack.Data;
using SljemeTimeAttack.Models;

namespace SljemeTimeAttack.Repos
{
    public class SuspensionEfRepository
    {
        private readonly SljemeTimeAttackDbContext _context;

        public SuspensionEfRepository(SljemeTimeAttackDbContext context)
        {
            _context = context;
        }

        public void Add(Suspension suspension)
        {
            _context.Suspensions.Add(suspension);
            _context.SaveChanges();
        }
    }
}
