using System.Collections.Generic;

namespace ProducerFrameworkMod.ContentPack;

public abstract class ProducerData
{
    public string ModUniqueID;
    public string ProducerName;
    public string ProducerQualifiedItemId;
    public List<string> AdditionalProducerNames = new();
    public List<string> AdditionalProducerQualifiedItemId = new();
    public List<string> OverrideMod = new();
}