def read_loc(path):
    with open(path, "r", encoding="utf-8") as f:
        lines = f.readlines()

    ret = []
    for line in lines:
        if "=" not in line:
            continue
        lines = line.split("//")
        lines[0] = lines[0].split("=", 1)
        lines = lines[0] + lines[1:]
        lines = [line.strip() for line in lines]
        if lines[1] == "暂未翻译":
            lines[1] = None
        if len(lines) == 2:
            lines.append(None)
        ret.append(lines)
    return ret


def write_loc(path, loc, lang):
    s = "Localization\n{\n    " + lang + "\n    {\n"
    tab_str = "        "
    loc_str_list = []
    for line in loc:
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
    # zh_loc = [[it[0], it[2] if it[1] is None else it[1], None] for it in zh_loc]
    # write_loc('./XYZLocalization/Localization/RP-1/Contracts/zh-cn-temp.cfg', zh_loc, "zh-cn")

    zh_loc = read_loc("./XYZLocalization/Localization/RP-1/Contracts/zh-cn-temp.cfg")
    en_loc = read_loc("./XYZLocalization/Localization/RP-1/Contracts/en-us.cfg")
    for i in range(len(zh_loc)):
        zh_loc[i][2] = en_loc[i][1]
    write_loc(
        "./XYZLocalization/Localization/RP-1/Contracts/zh-cn-temp2.cfg", zh_loc, "zh-cn"
    )
