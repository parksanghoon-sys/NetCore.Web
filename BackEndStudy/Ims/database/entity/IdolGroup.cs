using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ims.database.entity;

public class IdolGroup
{
    [Key]
    public int IdolGroupId {get;set;}

    public required string GroupName {get;set;}

    public required DateTime CreateDateTime {get;set;} = DateTime.Now;

    public ICollection<Member> Members { get; set; }
}
