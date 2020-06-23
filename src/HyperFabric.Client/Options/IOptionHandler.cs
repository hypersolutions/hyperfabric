namespace HyperFabric.Client.Options
{
    public interface IOptionHandler
    {
        void Handle(DeploymentOptions deploymentOption);
    }
}
