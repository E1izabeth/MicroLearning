export class OkType {
}
export class ResourceFilterSpecParametersBaseType {
        _type: string;
}
export class ResourceFilterByKeywordsSpec extends ResourceFilterSpecParametersBaseType {
        AssociationTags: Array<AssociationTagInfoType>;
}
export class AssociationTagInfoType {
        Word: string;
        Id: Int64;
}
export type Int64 = number;
export class ResourceFilterByTopicSpec extends ResourceFilterSpecParametersBaseType {
        TopicId: Int64;
}
export class RegisterSpecType {
        Login: string;
        Password: string;
        Email: string;
}
export class RequestActivationSpecType {
        Email: string;
}
export class ChangePasswordSpecType {
        NewPassword: string;
        Email: string;
}
export class ChangeEmailSpecType {
        Password: string;
        OldEmail: string;
        NewEmail: string;
}
export class ResetPasswordSpecType {
        Login: string;
        Email: string;
}
export class LoginSpecType {
        Login: string;
        Password: string;
}
export class ProfileFootprintInfoType {
        Login: string;
        EmailFootprint: string;
        IsActivated: boolean;
}
export class CreateTopicSpecType {
        TopicName: string;
        AssociationTags: Array<AssociationTagInfoType>;
}
export class TopicsListType {
        Count: Int32;
        Items: Array<TopicInfoType>;
}
export type Int32 = number;
export class TopicInfoType {
        Title: string;
        Id: Int64;
        TotalContentParts: Int64;
        LearnedContentParts: Int64;
        IsActive: boolean;
        DeliveryIntervalSeconds: Int64;
        AssociationTags: Array<AssociationTagInfoType>;
}
export class ActivateTopicSpecType {
        DueSeconds: Int64;
        IntervalSeconds: Int64;
}
export type ResourceFilterSpecType_Item = ResourceFilterByKeywordsSpec | ResourceFilterByTopicSpec;
export class ResourceFilterSpecType {
        Item: ResourceFilterSpecType_Item;
}
export class CreateResourceSpecType {
        ResourceTitle: string;
        ResourceUrl: string;
        AssociationTags: Array<AssociationTagInfoType>;
}
export class ResourcesListType {
        Count: Int32;
        Items: Array<ResourceInfoType>;
}
export class ResourceInfoType {
        Title: string;
        Url: string;
        IsValidated: boolean;
        IsMyResource: boolean;
        Id: Int64;
        AssociationTags: Array<AssociationTagInfoType>;
}
export class ContentPartsListType {
        Count: Int32;
        Items: Array<ContentPartInfoType>;
}
export class ContentPartInfoType {
        Order: Int64;
        ResourceId: Int64;
        Id: Int64;
        IsDelivered: boolean;
        IsLearned: boolean;
        Text: string;
}
export class ErrorInfoType {
        TypeName: string;
        Message: string;
        StackTrace: string;
        InnerError: ErrorInfoType;
}
export class ExtendedErrorInfoType {
        TypeName: string;
        RawErrorInfo: string;
        Message: string;
        StackTrace: string;
        InnerError: ErrorInfoType;
}
