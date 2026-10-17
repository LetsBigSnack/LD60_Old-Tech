namespace Data
{
    public interface IPunchcardReceiver
    {
        bool InsertPunchcard(PunchCardData card);
        void RemovePunchcard();
    }
}