


namespace DataSystem.Models.ViewModels.chart  {
    
    public class allProvince{

	public string PROV_32_ID{get;set;}
	public string Pname{get;set;}
	public int Value{get;set;}
	public int Children6m{get;set;}
	public double Children6mp{get;set;}
	public int Children23m{get;set;}
	public double Children23mp{get;set;}
	public int Children59m{get;set;}
	public double Children59mp{get;set;}
	public int Discharge{get;set;}
    public double DischargeP{get;set;}
	public int Cured{get;set;}
	public double CuredP{get;set;}
	public int Deaths{get;set;}
	public double DeathP{get;set;}
	public int Defaulter{get;set;}
	public double DefaultP{get;set;}

	public int Male{get;set;}
	public double MaleP{get;set;}
	public int Female{get;set;}
	public double Femalep {get;set;}
	public int Odema{get;set;}
	public int Zscore23 {get;set;}
	public int MUAC115{get;set;}
	public int MUAC12{get;set;}
	public int MUAC23{get;set;}
	public int ReferIn{get;set;}
	public int ReferOuts{get;set;}
	public int NonCured{get;set;}
	public double NonCuredP{get;set;}

    }
}