namespace uni_elastic_manager.infra
{
    ///<summary>A contract for any method used to call ARIMA, which may be any of them using R.</summary>  
    public interface IArima
    {
        double Calculate(string[] metrics);
    }
}