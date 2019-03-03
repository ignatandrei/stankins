using System;
//generated with https://app.quicktype.io/
namespace Stankins.Trello
{
    partial class TrelloJSON
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public object DescData { get; set; }
        public bool Closed { get; set; }
        public object IdOrganization { get; set; }
        public WelcomeLimits Limits { get; set; }
        public bool Pinned { get; set; }
        public object Starred { get; set; }
        public Uri Url { get; set; }
        public WelcomePrefs Prefs { get; set; }
        public string ShortLink { get; set; }
        public object Subscribed { get; set; }
        public LabelNames LabelNames { get; set; }
        public object[] PowerUps { get; set; }
        public DateTimeOffset DateLastActivity { get; set; }
        public object DateLastView { get; set; }
        public Uri ShortUrl { get; set; }
        public object[] IdTags { get; set; }
        public object DatePluginDisable { get; set; }
        public object CreationMethod { get; set; }
        public Action[] Actions { get; set; }
        public CardElement[] Cards { get; set; }
        public Label[] Labels { get; set; }
        public ListElement[] Lists { get; set; }
        public Member[] Members { get; set; }
        public object[] Checklists { get; set; }
        public object[] CustomFields { get; set; }
        public Membership[] Memberships { get; set; }
        public PluginDatum[] PluginData { get; set; }
    }

    partial class Action
    {
        public string Id { get; set; }
        public string IdMemberCreator { get; set; }
        public Data Data { get; set; }
        public string Type { get; set; }
        public DateTimeOffset Date { get; set; }
        public ActionLimits Limits { get; set; }
        public MemberCreator MemberCreator { get; set; }
    }

    partial class Data
    {
        public DataList List { get; set; }
        public Board Board { get; set; }
        public DataCard Card { get; set; }
        public string Text { get; set; }
        public DataAttachment Attachment { get; set; }
        public Old Old { get; set; }
        public TextDataClass TextData { get; set; }
        public DateTimeOffset? DateLastEdited { get; set; }
        public List ListAfter { get; set; }
        public List ListBefore { get; set; }
        public Plugin Plugin { get; set; }
        public BoardSource BoardSource { get; set; }
    }

    partial class DataAttachment
    {
        public Uri Url { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string EdgeColor { get; set; }
        public Uri PreviewUrl { get; set; }
        public Uri PreviewUrl2X { get; set; }
    }

    partial class Board
    {
        public string ShortLink { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public BoardPrefs Prefs { get; set; }
    }

    partial class BoardPrefs
    {
        public string PermissionLevel { get; set; }
        public bool? CardCovers { get; set; }
    }

    partial class BoardSource
    {
        public string Id { get; set; }
    }

    partial class DataCard
    {
        public string ShortLink { get; set; }
        public long IdShort { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string Desc { get; set; }
        public bool? Closed { get; set; }
        public double? Pos { get; set; }
        public string IdList { get; set; }
    }

    partial class DataList
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public long? Pos { get; set; }
        public bool? Closed { get; set; }
    }

    partial class List
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    partial class Old
    {
        public string Desc { get; set; }
        public string Name { get; set; }
        public bool? Closed { get; set; }
        public long? Pos { get; set; }
        public string IdList { get; set; }
        public BoardPrefs Prefs { get; set; }
    }

    partial class Plugin
    {
        public Listing Listing { get; set; }
        public Icon Icon { get; set; }
        public bool IsCompliantWithPrivacyStandards { get; set; }
        public object[] Tags { get; set; }
        public Uri Url { get; set; }
        public object ModeratedState { get; set; }
        public bool Public { get; set; }
        public string Name { get; set; }
        public Uri IframeConnectorUrl { get; set; }
        public string[] Categories { get; set; }
        public string[] Capabilities { get; set; }
        public string Author { get; set; }
        public string IdOrganizationOwner { get; set; }
        public string Id { get; set; }
        public string SupportEmail { get; set; }
        public string PrivacyUrl { get; set; }
    }

    partial class Icon
    {
        public Uri Url { get; set; }
    }

    partial class Listing
    {
        public string Overview { get; set; }
        public string Description { get; set; }
        public string Locale { get; set; }
        public string Name { get; set; }
    }

    partial class TextDataClass
    {
        public NonPublic Emoji { get; set; }
    }

    partial class NonPublic
    {
    }

    partial class ActionLimits
    {
        public Reactions Reactions { get; set; }
    }

    partial class Reactions
    {
        public PerBoard PerAction { get; set; }
        public PerBoard UniquePerAction { get; set; }
    }

    partial class PerBoard
    {
        public string Status { get; set; }
        public long DisableAt { get; set; }
        public long WarnAt { get; set; }
    }

    partial class MemberCreator
    {
        public string Id { get; set; }
        public string AvatarHash { get; set; }
        public Uri AvatarUrl { get; set; }
        public string FullName { get; set; }
        public object IdMemberReferrer { get; set; }
        public string Initials { get; set; }
        public NonPublic NonPublic { get; set; }
        public string Username { get; set; }
    }

    partial class CardElement
    {
        public string Id { get; set; }
        public object Address { get; set; }
        public object CheckItemStates { get; set; }
        public bool Closed { get; set; }
        public object Coordinates { get; set; }
        public object CreationMethod { get; set; }
        public DateTimeOffset DateLastActivity { get; set; }
        public string Desc { get; set; }
        public TextDataClass DescData { get; set; }
        public string IdBoard { get; set; }
        public string[] IdLabels { get; set; }
        public string IdList { get; set; }
        public object[] IdMembersVoted { get; set; }
        public long IdShort { get; set; }
        public string IdAttachmentCover { get; set; }
        public CardLimits Limits { get; set; }
        public object LocationName { get; set; }
        public bool ManualCoverAttachment { get; set; }
        public string Name { get; set; }
        public double Pos { get; set; }
        public string ShortLink { get; set; }
        public Badges Badges { get; set; }
        public bool DueComplete { get; set; }
        public object Due { get; set; }
        public object[] IdChecklists { get; set; }
        public object[] IdMembers { get; set; }
        public Label[] Labels { get; set; }
        public Uri ShortUrl { get; set; }
        public bool Subscribed { get; set; }
        public Uri Url { get; set; }
        public AttachmentElement[] Attachments { get; set; }
        public object[] PluginData { get; set; }
        public object[] CustomFieldItems { get; set; }
    }

    partial class AttachmentElement
    {
        public long? Bytes { get; set; }
        public DateTimeOffset Date { get; set; }
        public string EdgeColor { get; set; }
        public string IdMember { get; set; }
        public bool IsUpload { get; set; }
        public string MimeType { get; set; }
        public string Name { get; set; }
        public Preview[] Previews { get; set; }
        public Uri Url { get; set; }
        public long Pos { get; set; }
        public string Id { get; set; }
    }

    partial class Preview
    {
        public long Bytes { get; set; }
        public Uri Url { get; set; }
        public long Height { get; set; }
        public long Width { get; set; }
        public string Id { get; set; }
        public bool Scaled { get; set; }
    }

    partial class Badges
    {
        public AttachmentsByType AttachmentsByType { get; set; }
        public bool Location { get; set; }
        public long Votes { get; set; }
        public bool ViewingMemberVoted { get; set; }
        public bool Subscribed { get; set; }
        public string Fogbugz { get; set; }
        public long CheckItems { get; set; }
        public long CheckItemsChecked { get; set; }
        public long Comments { get; set; }
        public long Attachments { get; set; }
        public bool Description { get; set; }
        public object Due { get; set; }
        public bool DueComplete { get; set; }
    }

    partial class AttachmentsByType
    {
        public Trello Trello { get; set; }
    }

    partial class Trello
    {
        public long Board { get; set; }
        public long Card { get; set; }
    }

    partial class Label
    {
        public string Id { get; set; }
        public string IdBoard { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }

    partial class CardLimits
    {
        public Stickers Attachments { get; set; }
        public Stickers Checklists { get; set; }
        public Stickers Stickers { get; set; }
    }

    partial class Stickers
    {
        public PerBoard PerCard { get; set; }
    }

    partial class LabelNames
    {
        public string Green { get; set; }
        public string Yellow { get; set; }
        public string Orange { get; set; }
        public string Red { get; set; }
        public string Purple { get; set; }
        public string Blue { get; set; }
        public string Sky { get; set; }
        public string Lime { get; set; }
        public string Pink { get; set; }
        public string Black { get; set; }
    }

    partial class WelcomeLimits
    {
        public Attachments Attachments { get; set; }
        public Boards Boards { get; set; }
        public PurpleCards Cards { get; set; }
        public Attachments Checklists { get; set; }
        public CheckItems CheckItems { get; set; }
        public CustomFields CustomFields { get; set; }
        public CustomFields Labels { get; set; }
        public Lists Lists { get; set; }
        public Stickers Stickers { get; set; }
        public Reactions Reactions { get; set; }
    }

    partial class Attachments
    {
        public PerBoard PerBoard { get; set; }
        public PerBoard PerCard { get; set; }
    }

    partial class Boards
    {
        public PerBoard TotalMembersPerBoard { get; set; }
    }

    partial class PurpleCards
    {
        public PerBoard OpenPerBoard { get; set; }
        public PerBoard OpenPerList { get; set; }
        public PerBoard TotalPerBoard { get; set; }
        public PerBoard TotalPerList { get; set; }
    }

    partial class CheckItems
    {
        public PerBoard PerChecklist { get; set; }
    }

    partial class CustomFields
    {
        public PerBoard PerBoard { get; set; }
    }

    partial class Lists
    {
        public PerBoard OpenPerBoard { get; set; }
        public PerBoard TotalPerBoard { get; set; }
    }

    partial class ListElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Closed { get; set; }
        public string IdBoard { get; set; }
        public long Pos { get; set; }
        public object Subscribed { get; set; }
        public object SoftLimit { get; set; }
        public ListLimits Limits { get; set; }
        public object CreationMethod { get; set; }
    }

    partial class ListLimits
    {
        public FluffyCards Cards { get; set; }
    }

    partial class FluffyCards
    {
        public PerBoard OpenPerList { get; set; }
        public PerBoard TotalPerList { get; set; }
    }

    partial class Member
    {
        public string Id { get; set; }
        public string AvatarHash { get; set; }
        public Uri AvatarUrl { get; set; }
        public string Bio { get; set; }
        public object BioData { get; set; }
        public bool Confirmed { get; set; }
        public string FullName { get; set; }
        public object IdEnterprise { get; set; }
        public object IdEnterprisesDeactivated { get; set; }
        public object IdMemberReferrer { get; set; }
        public object IdPremOrgsAdmin { get; set; }
        public string Initials { get; set; }
        public string MemberType { get; set; }
        public NonPublic NonPublic { get; set; }
        public object[] Products { get; set; }
        public string Status { get; set; }
        public Uri Url { get; set; }
        public string Username { get; set; }
    }

    partial class Membership
    {
        public string Id { get; set; }
        public string IdMember { get; set; }
        public string MemberType { get; set; }
        public bool Unconfirmed { get; set; }
    }

    partial class PluginDatum
    {
        public string Id { get; set; }
        public string IdPlugin { get; set; }
        public string Scope { get; set; }
        public string IdModel { get; set; }
        public string Value { get; set; }
        public string Access { get; set; }
    }

    partial class WelcomePrefs
    {
        public string PermissionLevel { get; set; }
        public string Voting { get; set; }
        public string Comments { get; set; }
        public string Invitations { get; set; }
        public bool SelfJoin { get; set; }
        public bool CardCovers { get; set; }
        public string CardAging { get; set; }
        public bool CalendarFeedEnabled { get; set; }
        public string Background { get; set; }
        public object BackgroundImage { get; set; }
        public object BackgroundImageScaled { get; set; }
        public bool BackgroundTile { get; set; }
        public string BackgroundBrightness { get; set; }
        public string BackgroundColor { get; set; }
        public string BackgroundBottomColor { get; set; }
        public string BackgroundTopColor { get; set; }
        public bool CanBePublic { get; set; }
        public bool CanBeEnterprise { get; set; }
        public bool CanBeOrg { get; set; }
        public bool CanBePrivate { get; set; }
        public bool CanInvite { get; set; }
    }

}
