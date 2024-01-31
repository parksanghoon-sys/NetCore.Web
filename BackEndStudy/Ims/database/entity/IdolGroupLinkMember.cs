using System.ComponentModel.DataAnnotations;

namespace Ims.database.entity;
public class IdolGroupLinkMember {
    [Key]
    public int IdolGroupId {get;set;}
    [Key]
    public int MemberId {get;set;}
}