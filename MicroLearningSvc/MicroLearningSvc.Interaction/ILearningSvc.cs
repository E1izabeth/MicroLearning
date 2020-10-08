using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningSvc.Interaction
{
    [ServiceContract(Namespace = "MicroLearningSvc")]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document)]
    public interface ILearningSvc
    {
        //[OperationContract, WebInvoke(UriTemplate = "/{*path}", Method = "OPTIONS")]
        //void CorsHandler(string path);

        [OperationContract, WebInvoke(UriTemplate = "/profile?action=register", Method = "POST")]
        void Register(RegisterSpecType registerSpec);
        [OperationContract, WebInvoke(UriTemplate = "/profile?action=restore", Method = "POST")]
        void RequestAccess(ResetPasswordSpecType spec);
        [OperationContract, WebGet(UriTemplate = "/profile?action=activate&key={key}")]
        OkType Activate(string key);
        [OperationContract, WebGet(UriTemplate = "/profile?action=restore&key={key}")]
        OkType RestoreAccess(string key);
        [OperationContract, WebInvoke(UriTemplate = "/profile?action=delete", Method = "POST")]
        void DeleteProfile();

        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "/profile?action=login")]
        void Login(LoginSpecType loginSpec);
        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "/profile?action=activate")]
        void RequestActivation(RequestActivationSpecType spec);
        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "/profile?action=set-email")]
        void SetEmail(ChangeEmailSpecType spec);
        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "/profile?action=set-password")]
        void SetPassword(ChangePasswordSpecType spec);
        [OperationContract, WebInvoke(Method = "POST", UriTemplate = "/profile?action=logout")]
        void Logout();
        [OperationContract, WebGet(UriTemplate = "/profile")]
        ProfileFootprintInfoType GetProfileFootprint();

        [OperationContract, WebInvoke(UriTemplate = "/resources?action=suggest", Method = "POST")]
        CreateResourceSpecType SuggestResourceTags(CreateResourceSpecType spec);
        [OperationContract, WebInvoke(UriTemplate = "/resources?action=create", Method = "POST")]
        ResourceInfoType CreateResource(CreateResourceSpecType spec);
        [OperationContract, WebGet(UriTemplate = "/resources/")]
        ResourcesListType GetResources();
        [OperationContract, WebGet(UriTemplate = "/resources/?from={skip}&count={take}")]
        ResourcesListType GetResourcesRange(string skip, string take);
        [OperationContract, WebInvoke(UriTemplate = "/resources/?from={skip}&count={take}", Method = "POST")]
        ResourcesListType FilterResourcesRange(string skip, string take, ResourceFilterSpecType filterSpec);
        [OperationContract, WebGet(UriTemplate = "/resources/{id}")]
        ResourceInfoType GetResource(string id);
        [OperationContract, WebInvoke(UriTemplate = "/resources/{id}?action=validate", Method = "POST")]
        void ValidateResource(string id);

        [OperationContract, WebInvoke(UriTemplate = "/topics?action=create", Method = "POST")]
        TopicInfoType CreateTopic(CreateTopicSpecType spec);
        [OperationContract, WebGet(UriTemplate = "/topics/")]
        TopicsListType GetTopics();
        [OperationContract, WebGet(UriTemplate = "/topics/?from={skip}&count={take}")]
        TopicsListType GetTopicsRange(string skip, string take);
        [OperationContract, WebGet(UriTemplate = "/topics/{id}")]
        TopicInfoType GetTopic(string id);
        [OperationContract, WebInvoke(UriTemplate = "/topics/{id}", Method = "DELETE")]
        void DeleteTopic(string id);
        [OperationContract, WebInvoke(UriTemplate = "/topics/{id}?action=reset", Method = "POST")]
        void ResetTopicLearningProgress(string id);
        [OperationContract, WebInvoke(UriTemplate = "/topics/{id}?action=activate", Method = "POST")]
        void ActivateTopicLearning(string id, ActivateTopicSpecType spec);
        [OperationContract, WebInvoke(UriTemplate = "/topics/{id}?action=deactivate", Method = "POST")]
        void DeactivateTopicLearning(string id);

        [OperationContract, WebGet(UriTemplate = "/topics/{id}/parts?from={skip}&count={take}")]
        ContentPartsListType GetContentPartsByTopic(string id, string skip, string take);
        //[OperationContract, WebGet(UriTemplate = "/resources/{id}/parts?from={skip}&count={take}")]
        //ContentPartsListType GetContentPartsByResource(string resourceIdStr, string skip, string take);
        [OperationContract, WebInvoke(UriTemplate = "/topics/{topicId}/parts/{partId}?action=mark", Method = "POST")]
        void MarkContentPartLearned(string topicId, string partId);
        [OperationContract, WebInvoke(UriTemplate = "/topics/{topicId}/parts/{partId}?action=unmark", Method = "POST")]
        void UnmarkContentPartLearned(string topicId, string partId);
    }
}
