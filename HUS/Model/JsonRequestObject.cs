using System;

public class Rootobject
{
    public int total { get; set; }
    public int limit { get; set; }
    public string dataset { get; set; }
    public Record[] records { get; set; }
}

public class Record
{
    public string ChargeOwner { get; set; }
    public string GLN_Number { get; set; }
    public string ChargeType { get; set; }
    public string ChargeTypeCode { get; set; }
    public string Note { get; set; }
    public string Description { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public string VATClass { get; set; }
    public float Price1 { get; set; }
    public float Price2 { get; set; }
    public float Price3 { get; set; }
    public float Price4 { get; set; }
    public float Price5 { get; set; }
    public float Price6 { get; set; }
    public float Price7 { get; set; }
    public float Price8 { get; set; }
    public float Price9 { get; set; }
    public float Price10 { get; set; }
    public float Price11 { get; set; }
    public float Price12 { get; set; }
    public float Price13 { get; set; }
    public float Price14 { get; set; }
    public float Price15 { get; set; }
    public float Price16 { get; set; }
    public float Price17 { get; set; }
    public float Price18 { get; set; }
    public float Price19 { get; set; }
    public float Price20 { get; set; }
    public float Price21 { get; set; }
    public float Price22 { get; set; }
    public float Price23 { get; set; }
    public float Price24 { get; set; }
    public int TransparentInvoicing { get; set; }
    public int TaxIndicator { get; set; }
    public string ResolutionDuration { get; set; }
}