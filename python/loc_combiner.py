def read_loc(path):
    with open(path, "r", encoding="utf-8") as f:
        lines = f.readlines()

    ret = []
    for line in lines:
        line = line.strip()
        if line.startswith("//"):
            triplet = [None, None, line[2:]]
        else:
            if '=' not in line:
                continue
            triplet = line.split("//")
            triplet[0] = triplet[0].split("=", 1)
            triplet = triplet[0] + triplet[1:]
            triplet = [it.strip() for it in triplet]
            if triplet[1] == "暂未翻译":
                triplet[1] = None
            if len(triplet) == 2:
                triplet.append(None)
        ret.append(triplet)
    return ret


def write_loc(path, loc, lang):
    s = "Localization\n{\n    " + lang + "\n    {\n"
    tab_str = "        "
    loc_str_list = []
    for line in loc:
        if line[0] is None:
            loc_str = tab_str + "// " + line[2] + "\n"
        else:
            if line[1] is None:
                line[1] = "暂未翻译"
            loc_str = tab_str + line[0] + " = " + line[1]
            if line[2] is not None:
                loc_str += " // " + line[2]
            loc_str += "\n"
        loc_str_list.append(loc_str)
    s += "".join(loc_str_list)
    s += "    }\n}"

    with open(path, "w", encoding="utf-8") as f:
        f.write(s)


if __name__ == "__main__":
    # zh_loc = read_loc('./XYZLocalization/Localization/RP-1/Contracts/zh-cn.cfg')
    # zh_loc = [[it[0], it[1], None] for it in zh_loc]
    # zh_loc = [[it[0], it[2] if it[1] is None else it[1], None] for it in zh_loc]
    # write_loc('./XYZLocalization/Localization/RP-1/Contracts/zh-cn-temp.cfg', zh_loc, "zh-cn")

    zh_loc = read_loc(r"./XYZLocalization\Localization\RealismOverhaul\Localization\zh-cn-Engines.cfg")
    en_loc = read_loc(r"./RealismOverhaul\Localization\en-us-Engines.cfg")
    for i in range(len(zh_loc)):
        if zh_loc[i][0] is not None:
            zh_loc[i][2] = en_loc[i][1]
    write_loc(
        r"./XYZLocalization\Localization\RealismOverhaul\Localization\zh-cn-Engines-temp2.cfg", zh_loc, "zh-cn"
    )