namespace HUS.Tests;

public class OptimizerTest
{
    //public _solver _solver = _solver.Create_solver("TEST__solver");
    private Solver _solver = new Solver("name", Solver.OptimizationProblemType.SCIP_MIXED_INTEGER_PROGRAMMING);
    
    [Theory]
    [InlineData("1/5/2024 15:00", "1/5/2024 15:00", 1.1, 1.1, "summer")]
    [InlineData("1/5/2024 15:00", "1/5/2024 15:00", 3.2, 3.1, "summer")]
    [InlineData("1/5/2024 15:00", "1/5/2024 15:00", 7.5, 5.1, "summer")]
    [InlineData("1/5/2024 15:00", "1/5/2024 15:00", 2.4, 2.1, "summer")]
    [InlineData("1/5/2024 15:00", "1/5/2024 15:00", 8.1, 7.3, "summer")]
    [InlineData("1/5/2024 15:00", "1/5/2024 15:00", 6, 4.2, "winter")]
    [InlineData("1/5/2024 15:00", "1/5/2024 15:00", 3, 2.1, "winter")]
    [InlineData("1/5/2024 15:00", "1/5/2024 15:00", 9.6, 2.7, "winter")]
    [InlineData("1/5/2024 15:00", "1/5/2024 15:00", 11.2, 4.5, "winter")]
    [InlineData("1/5/2024 15:00", "1/5/2024 15:00", 2.1, 9.1, "winter")]
    public void Test(string hourStart, string hourEnd, double demand, double electricityPrice, string period)
    {
        
        Optimizer optimizer = new Optimizer(hourStart, hourEnd, demand, electricityPrice, period);
        // Assert
        //_solver _solver = _solver.Create_solver("TEST__solver");
        Variable gb = _solver.MakeNumVar(0.0, 5.0, "GasBoiler");
        Variable ob = _solver.MakeNumVar(0.0, 4.0, "OilBoiler");
        Variable gm = _solver.MakeNumVar(0.0, 3.6, "GasMotor");
        Variable ek = _solver.MakeNumVar(0.0, 8.0, "ElectricBoiler");
        _solver.Add(gb + ob + gm >= demand);
        Objective objective = _solver.Objective();
        objective.SetMinimization();
        objective.SetCoefficient(gb, 500);
        objective.SetCoefficient(ob, 700);
        objective.SetCoefficient(gm, 1100);
        objective.SetCoefficient(ek, 50);
        Solver.ResultStatus resultStatus = _solver.Solve();
        
        Assert.Equal(gb.SolutionValue(), optimizer.GasBoilerOutput);
        Assert.Equal(gm.SolutionValue(), optimizer.GasMotorOutput);
        Assert.Equal(ek.SolutionValue(), optimizer.ElectricBoilerOutput);
        Assert.Equal(ob.SolutionValue(), optimizer.OilBoilerOutput);
    }
 
}