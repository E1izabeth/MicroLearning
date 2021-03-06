
    
  public interface IResourceFilterSpecParametersBaseTypeVisitor<T>
  {
    
    T VisitResourceFilterByTopicSpec(ResourceFilterByTopicSpec node);
      
    T VisitResourceFilterByKeywordsSpec(ResourceFilterByKeywordsSpec node);
      
  }

  abstract partial class ResourceFilterSpecParametersBaseType
  {
    public T Apply<T>(IResourceFilterSpecParametersBaseTypeVisitor<T> visitor)
    {
      return this.ApplyImpl<T>(visitor);
    }
      
    protected abstract T ApplyImpl<T>(IResourceFilterSpecParametersBaseTypeVisitor<T> visitor);
  }

    
  partial class ResourceFilterByTopicSpec
  {
    protected override T ApplyImpl<T>(IResourceFilterSpecParametersBaseTypeVisitor<T> visitor)
    {
      return visitor.VisitResourceFilterByTopicSpec(this);
    }
  }
      
  partial class ResourceFilterByKeywordsSpec
  {
    protected override T ApplyImpl<T>(IResourceFilterSpecParametersBaseTypeVisitor<T> visitor)
    {
      return visitor.VisitResourceFilterByKeywordsSpec(this);
    }
  }
      