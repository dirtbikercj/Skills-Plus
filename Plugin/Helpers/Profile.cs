using System.Collections.Generic;

namespace SkillsExtended.Helpers
{
    public class SkillProgress
    {
        public string SkillId { get; set; }
        public float Progress { get; set; }
    }

    public class Profile
    {
        public string ProfileId { get; set; }
        public List<SkillProgress> Skills { get; set; }
    }
}
