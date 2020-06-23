namespace HyperFabric.Client.Options
{
    public sealed class JsonOptionHandler : IOptionHandler
    {
        public void Handle(DeploymentOptions deploymentOption)
        {
            var json = deploymentOption.Json.Trim();
            
            if (json.StartsWith("{") && json.EndsWith("}"))
            {
                deploymentOption.JsonString = json;
            }
            else
            {
                deploymentOption.JsonPath = json;
            }
        }
    }
}
