namespace Game;

struct Tag
{
    public string tag;

    public Tag(string tag) => this.tag = tag;

    public static bool operator ==(Tag tag, string str) => tag.tag == str;

    public static bool operator !=(Tag tag, string str) => tag.tag != str;
}
