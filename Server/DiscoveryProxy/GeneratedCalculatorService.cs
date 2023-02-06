﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------



[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(Namespace="http://hsfl.ceho5518.vs.server.DiscoveryProxy", ConfigurationName="ICalculatorService")]
public interface ICalculatorService
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/Add", ReplyAction="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/AddResponse")]
    double Add(double n1, double n2);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/Add", ReplyAction="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/AddResponse")]
    System.Threading.Tasks.Task<double> AddAsync(double n1, double n2);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/Subtract", ReplyAction="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/SubtractResponse" +
        "")]
    double Subtract(double n1, double n2);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/Subtract", ReplyAction="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/SubtractResponse" +
        "")]
    System.Threading.Tasks.Task<double> SubtractAsync(double n1, double n2);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/Multiply", ReplyAction="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/MultiplyResponse" +
        "")]
    double Multiply(double n1, double n2);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/Multiply", ReplyAction="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/MultiplyResponse" +
        "")]
    System.Threading.Tasks.Task<double> MultiplyAsync(double n1, double n2);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/Divide", ReplyAction="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/DivideResponse")]
    double Divide(double n1, double n2);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/Divide", ReplyAction="http://hsfl.ceho5518.vs.server.DiscoveryProxy/ICalculatorService/DivideResponse")]
    System.Threading.Tasks.Task<double> DivideAsync(double n1, double n2);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface ICalculatorServiceChannel : ICalculatorService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class CalculatorServiceClient : System.ServiceModel.ClientBase<ICalculatorService>, ICalculatorService
{
    
    public CalculatorServiceClient()
    {
    }
    
    public CalculatorServiceClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public CalculatorServiceClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public CalculatorServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public CalculatorServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public double Add(double n1, double n2)
    {
        return base.Channel.Add(n1, n2);
    }
    
    public System.Threading.Tasks.Task<double> AddAsync(double n1, double n2)
    {
        return base.Channel.AddAsync(n1, n2);
    }
    
    public double Subtract(double n1, double n2)
    {
        return base.Channel.Subtract(n1, n2);
    }
    
    public System.Threading.Tasks.Task<double> SubtractAsync(double n1, double n2)
    {
        return base.Channel.SubtractAsync(n1, n2);
    }
    
    public double Multiply(double n1, double n2)
    {
        return base.Channel.Multiply(n1, n2);
    }
    
    public System.Threading.Tasks.Task<double> MultiplyAsync(double n1, double n2)
    {
        return base.Channel.MultiplyAsync(n1, n2);
    }
    
    public double Divide(double n1, double n2)
    {
        return base.Channel.Divide(n1, n2);
    }
    
    public System.Threading.Tasks.Task<double> DivideAsync(double n1, double n2)
    {
        return base.Channel.DivideAsync(n1, n2);
    }
}