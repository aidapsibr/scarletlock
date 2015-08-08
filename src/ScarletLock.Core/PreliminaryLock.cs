namespace ScarletLock
{
    public class PreliminaryLock<TIdentity>
    {
        public PreliminaryLock(TIdentity identity, string resource)
        {
            Identity = identity;
            Resource = resource;
        }

        public TIdentity Identity { get; }

        public string Resource { get; }
    }
}
