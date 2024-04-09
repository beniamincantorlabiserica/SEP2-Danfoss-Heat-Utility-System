using System;
using System.Collections.Generic;
using Google.OrTools.LinearSolver;

namespace HUS.Model;

public class Optimizer
{
    public string HourStart { get; set; }
    public string HourEnd { get; set; }
    public double Demand { get; set; }
    public double ElectricityPrice { get; set; }
    public string Period { get; set; }
    public double GasBoilerOutput { get; private set; }
    public double OilBoilerOutput { get; private set; }
    public double GasMotorOutput { get; private set; }
    public double ElectricBoilerOutput { get; private set; }
    public double TotalCost { get; private set; }

    public Optimizer(string hourStart, string hourEnd, double demand, double electricityPrice, string period)
    {
        HourStart = hourStart;
        HourEnd = hourEnd;
        Demand = demand;
        ElectricityPrice = electricityPrice;
        Period = period;

        Optimize();
    }

    private void Optimize()
    {
        Solver solver = Solver.CreateSolver("SCIP");

        Variable gb = solver.MakeNumVar(0.0, 5.0, "GasBoiler");
        Variable ob = solver.MakeNumVar(0.0, 4.0, "OilBoiler");
        Variable gm = solver.MakeNumVar(0.0, 3.6, "GasMotor");
        Variable ek = solver.MakeNumVar(0.0, 8.0, "ElectricBoiler");

        solver.Add(gb + ob + gm + ek >= Demand);

        Objective objective = solver.Objective();
        objective.SetMinimization();
        objective.SetCoefficient(gb, 500);
        objective.SetCoefficient(ob, 700);
        objective.SetCoefficient(gm, 1100);
        objective.SetCoefficient(ek, 50);

        Solver.ResultStatus resultStatus = solver.Solve();

        if (resultStatus == Solver.ResultStatus.OPTIMAL)
        {
            GasBoilerOutput = gb.SolutionValue();
            OilBoilerOutput = ob.SolutionValue();
            GasMotorOutput = gm.SolutionValue();
            ElectricBoilerOutput = ek.SolutionValue();
            TotalCost = solver.Objective().Value();

            Console.WriteLine("--------------------------------------");
            Console.WriteLine("OPTIMIZING NEW DATASET");
            Console.Write("Gas Boiler: " + GasBoilerOutput + " Oil Boiler: " + OilBoilerOutput + " Gas Motor: " +
                          GasMotorOutput + " Electric Boiler: " + ElectricBoilerOutput + " Total Cost: " + TotalCost);
            Console.WriteLine("\n--------------------------------------");
            Console.WriteLine(" \n\n\n\n\n");
        }
        else
        {
            throw new Exception("The problem does not have an optimal solution.");
        }
    }
    
    public Dictionary<string, double> GetProductionUnits()
    {
        Dictionary<string, double> productionUnits = new Dictionary<string, double>
        {
            { "GasBoiler", GasBoilerOutput },
            { "OilBoiler", OilBoilerOutput },
            { "GasMotor", GasMotorOutput },
            { "ElectricBoiler", ElectricBoilerOutput }
        };
        return productionUnits;
    }
    
    public double GetTotalCost()
    {
        return TotalCost;
    }
}