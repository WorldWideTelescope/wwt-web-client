using System.Collections.Generic;


namespace wwtlib
{

    public class ColorMapContainer
    {

        // This class is intended to be used to store colormaps. It does not handle any
        // interpolation and when using FindClosestColor it will simply check which
        // color is closest to the requested value. Therefore, continuous colormaps should
        // be created by providing a sufficient number of colors (ideally 256 or more).

        public List<Color> colors = new List<Color>();

        public static ColorMapContainer FromNestedLists(List<List<float>> color_list)
        {
              
            // Class method to create a new colormap from a list of [r, g, b, a] lists.

            ColorMapContainer temp = new ColorMapContainer();
            foreach (List<float> color in color_list)
            {
                temp.colors.Add(Color.FromArgb(color[3], color[0], color[1], color[2]));
            }
            return temp;
        }

        public static ColorMapContainer FromStringList(List<string> color_list)
        {

            // Class method to create a new colormap from a list of strings.

            ColorMapContainer temp = new ColorMapContainer();
            foreach (string color in color_list)
            {
                temp.colors.Add(Color.Load(color));
            }
            return temp;
        }

        public Color FindClosestColor(float value)
        {
            // Given a floating-point value in the range 0 to 1, find the color that is the
            // closest to it.

            int index;

            if (value <= 0) {
                return colors[0];
            } else if (value >= 1) {
                return colors[colors.Count - 1];
            } else {
                index = (int)(value * colors.Count);
                return colors[index];
            }

        }

        public static ColorMapContainer FromNamedColormap(string name)
        {

            switch (name.ToLowerCase())
            {
                case "viridis":
                    return Viridis;
                case "plasma":
                    return Plasma;
                case "inferno":
                    return Inferno;
                case "magma":
                    return Magma;
                case "cividis":
                    return Cividis;
                case "Greys":
                    return Greys;
                case "Purples":
                    return Purples;
                case "Blues":
                    return Blues;
                case "Greens":
                    return Greens;
                case "Oranges":
                    return Oranges;
                case "Reds":
                    return Reds;
                case "RdYlBu":
                    return RdYlBu;
                return null;
            }
            return null;
        }

        // The colormaps below were produced using the following Python code:
        //
        // import numpy as np
        // from matplotlib import cm
        // from matplotlib.colors import to_hex
        // from textwrap import wrap, indent
        //
        // TEMPLATE = """
        // public static ColorMapContainer {name} = ColorMapContainer.FromStringList(new List<string>(
        // {colors}
        // ));
        // """
        //
        // TEMPLATE_CASE = """
        // case "{name_lower}":
        //     return {name};"""
        // COLORMAPS = ["viridis", "plasma", "inferno", "magma", "cividis",
        //             "Greys", "Purples", "Blues", "Greens", "Oranges", "Reds", "RdYlBu"]
        //
        // named_code = ""
        // case_code = ""
        //
        // for name in COLORMAPS:
        //     cmap = cm.get_cmap(name)
        //     x = np.linspace(0.5 / 256, 245.5/256, 256)
        //     colors = ", ".join(['"{0}"'.format(to_hex(c)) for c in cmap(x)])
        //     pretty_name = name[0].upper() + name[1:]
        //     named_code += TEMPLATE.format(name=pretty_name, colors=indent('\n'.join(wrap(colors, 90)), " " *  10))
        //     case_code += TEMPLATE_CASE.format(name=pretty_name, name_lower=name)
        //
        // named_code = indent(named_code, " " * 8)
        //
        // print(named_code)    
        // print('-' * 72)
        // print(case_code)

        public static ColorMapContainer Viridis = ColorMapContainer.FromStringList(new List<string>(
                "#440154", "#440256", "#450457", "#450559", "#46075a", "#46085c", "#460a5d", "#460b5e",
                "#470d60", "#470e61", "#471063", "#471164", "#471365", "#471365", "#481467", "#481668",
                "#481769", "#48186a", "#481a6c", "#481b6d", "#481c6e", "#481d6f", "#481f70", "#482071",
                "#482173", "#482374", "#482475", "#482576", "#482677", "#482878", "#482979", "#472a7a",
                "#472c7a", "#472d7b", "#472e7c", "#472f7d", "#46307e", "#46327e", "#46337f", "#46337f",
                "#463480", "#453581", "#453781", "#453882", "#443983", "#443a83", "#443b84", "#433d84",
                "#433e85", "#423f85", "#424086", "#424186", "#414287", "#414487", "#404588", "#404688",
                "#3f4788", "#3f4889", "#3e4989", "#3e4a89", "#3e4c8a", "#3d4d8a", "#3d4e8a", "#3c4f8a",
                "#3c4f8a", "#3c508b", "#3b518b", "#3b528b", "#3a538b", "#3a548c", "#39558c", "#39568c",
                "#38588c", "#38598c", "#375a8c", "#375b8d", "#365c8d", "#365d8d", "#355e8d", "#355f8d",
                "#34608d", "#34618d", "#33628d", "#33638d", "#32648e", "#32658e", "#31668e", "#31678e",
                "#31688e", "#30698e", "#30698e", "#306a8e", "#2f6b8e", "#2f6c8e", "#2e6d8e", "#2e6e8e",
                "#2e6f8e", "#2d708e", "#2d718e", "#2c718e", "#2c728e", "#2c738e", "#2b748e", "#2b758e",
                "#2a768e", "#2a778e", "#2a788e", "#29798e", "#297a8e", "#297b8e", "#287c8e", "#287d8e",
                "#277e8e", "#277f8e", "#27808e", "#27808e", "#26818e", "#26828e", "#26828e", "#25838e",
                "#25848e", "#25858e", "#24868e", "#24878e", "#23888e", "#23898e", "#238a8d", "#228b8d",
                "#228c8d", "#228d8d", "#218e8d", "#218f8d", "#21908d", "#21918c", "#20928c", "#20928c",
                "#20938c", "#1f948c", "#1f958b", "#1f968b", "#1f978b", "#1f978b", "#1f988b", "#1f998a",
                "#1f9a8a", "#1e9b8a", "#1e9c89", "#1e9d89", "#1f9e89", "#1f9f88", "#1fa088", "#1fa188",
                "#1fa187", "#1fa287", "#20a386", "#20a486", "#21a585", "#21a685", "#22a785", "#22a884",
                "#23a983", "#24aa83", "#25ab82", "#25ac82", "#26ad81", "#27ad81", "#27ad81", "#28ae80",
                "#29af7f", "#2ab07f", "#2cb17e", "#2db27d", "#2eb37c", "#2fb47c", "#31b57b", "#32b67a",
                "#34b679", "#35b779", "#37b878", "#38b977", "#3aba76", "#3bbb75", "#3dbc74", "#3fbc73",
                "#40bd72", "#42be71", "#44bf70", "#46c06f", "#48c16e", "#4ac16d", "#4cc26c", "#4ec36b",
                "#4ec36b", "#50c46a", "#52c569", "#54c568", "#56c667", "#58c765", "#5ac864", "#5cc863",
                "#5ec962", "#60ca60", "#63cb5f", "#65cb5e", "#67cc5c", "#69cd5b", "#6ccd5a", "#6ece58",
                "#70cf57", "#73d056", "#75d054", "#77d153", "#7ad151", "#7cd250", "#7fd34e", "#81d34d",
                "#84d44b", "#84d44b", "#86d549", "#89d548", "#8bd646", "#8ed645", "#90d743", "#93d741",
                "#95d840", "#98d83e", "#9bd93c", "#9dd93b", "#a0da39", "#a2da37", "#a5db36", "#a8db34",
                "#aadc32", "#addc30", "#b0dd2f", "#b2dd2d", "#b5de2b", "#b8de29", "#bade28", "#bddf26",
                "#c0df25", "#c2df23", "#c5e021", "#c5e021", "#c8e020", "#cae11f", "#cde11d", "#d0e11c",
                "#d2e21b", "#d5e21a", "#d8e219", "#dae319", "#dde318", "#dfe318", "#e2e418", "#e5e419"
        ));

        public static ColorMapContainer Plasma = ColorMapContainer.FromStringList(new List<string>(
                "#0d0887", "#100788", "#130789", "#16078a", "#19068c", "#1b068d", "#1d068e", "#20068f",
                "#220690", "#240691", "#260591", "#280592", "#2a0593", "#2a0593", "#2c0594", "#2e0595",
                "#2f0596", "#310597", "#330597", "#350498", "#370499", "#38049a", "#3a049a", "#3c049b",
                "#3e049c", "#3f049c", "#41049d", "#43039e", "#44039e", "#46039f", "#48039f", "#4903a0",
                "#4b03a1", "#4c02a1", "#4e02a2", "#5002a2", "#5102a3", "#5302a3", "#5502a4", "#5502a4",
                "#5601a4", "#5801a4", "#5901a5", "#5b01a5", "#5c01a6", "#5e01a6", "#6001a6", "#6100a7",
                "#6300a7", "#6400a7", "#6600a7", "#6700a8", "#6900a8", "#6a00a8", "#6c00a8", "#6e00a8",
                "#6f00a8", "#7100a8", "#7201a8", "#7401a8", "#7501a8", "#7701a8", "#7801a8", "#7a02a8",
                "#7a02a8", "#7b02a8", "#7d03a8", "#7e03a8", "#8004a8", "#8104a7", "#8305a7", "#8405a7",
                "#8606a6", "#8707a6", "#8808a6", "#8a09a5", "#8b0aa5", "#8d0ba5", "#8e0ca4", "#8f0da4",
                "#910ea3", "#920fa3", "#9410a2", "#9511a1", "#9613a1", "#9814a0", "#99159f", "#9a169f",
                "#9c179e", "#9d189d", "#9d189d", "#9e199d", "#a01a9c", "#a11b9b", "#a21d9a", "#a31e9a",
                "#a51f99", "#a62098", "#a72197", "#a82296", "#aa2395", "#ab2494", "#ac2694", "#ad2793",
                "#ae2892", "#b02991", "#b12a90", "#b22b8f", "#b32c8e", "#b42e8d", "#b52f8c", "#b6308b",
                "#b7318a", "#b83289", "#ba3388", "#ba3388", "#bb3488", "#bc3587", "#bd3786", "#be3885",
                "#bf3984", "#c03a83", "#c13b82", "#c23c81", "#c33d80", "#c43e7f", "#c5407e", "#c6417d",
                "#c7427c", "#c8437b", "#c9447a", "#ca457a", "#cb4679", "#cc4778", "#cc4977", "#cd4a76",
                "#ce4b75", "#cf4c74", "#d04d73", "#d14e72", "#d24f71", "#d24f71", "#d35171", "#d45270",
                "#d5536f", "#d5546e", "#d6556d", "#d7566c", "#d8576b", "#d9586a", "#da5a6a", "#da5b69",
                "#db5c68", "#dc5d67", "#dd5e66", "#de5f65", "#de6164", "#df6263", "#e06363", "#e16462",
                "#e26561", "#e26660", "#e3685f", "#e4695e", "#e56a5d", "#e56b5d", "#e56b5d", "#e66c5c",
                "#e76e5b", "#e76f5a", "#e87059", "#e97158", "#e97257", "#ea7457", "#eb7556", "#eb7655",
                "#ec7754", "#ed7953", "#ed7a52", "#ee7b51", "#ef7c51", "#ef7e50", "#f07f4f", "#f0804e",
                "#f1814d", "#f1834c", "#f2844b", "#f3854b", "#f3874a", "#f48849", "#f48948", "#f58b47",
                "#f58b47", "#f58c46", "#f68d45", "#f68f44", "#f79044", "#f79143", "#f79342", "#f89441",
                "#f89540", "#f9973f", "#f9983e", "#f99a3e", "#fa9b3d", "#fa9c3c", "#fa9e3b", "#fb9f3a",
                "#fba139", "#fba238", "#fca338", "#fca537", "#fca636", "#fca835", "#fca934", "#fdab33",
                "#fdac33", "#fdac33", "#fdae32", "#fdaf31", "#fdb130", "#fdb22f", "#fdb42f", "#fdb52e",
                "#feb72d", "#feb82c", "#feba2c", "#febb2b", "#febd2a", "#febe2a", "#fec029", "#fdc229",
                "#fdc328", "#fdc527", "#fdc627", "#fdc827", "#fdca26", "#fdcb26", "#fccd25", "#fcce25",
                "#fcd025", "#fcd225", "#fbd324", "#fbd324", "#fbd524", "#fbd724", "#fad824", "#fada24",
                "#f9dc24", "#f9dd25", "#f8df25", "#f8e125", "#f7e225", "#f7e425", "#f6e626", "#f6e826"
        ));

        public static ColorMapContainer Inferno = ColorMapContainer.FromStringList(new List<string>(
                "#000004", "#010005", "#010106", "#010108", "#02010a", "#02020c", "#02020e", "#030210",
                "#040312", "#040314", "#050417", "#060419", "#07051b", "#07051b", "#08051d", "#09061f",
                "#0a0722", "#0b0724", "#0c0826", "#0d0829", "#0e092b", "#10092d", "#110a30", "#120a32",
                "#140b34", "#150b37", "#160b39", "#180c3c", "#190c3e", "#1b0c41", "#1c0c43", "#1e0c45",
                "#1f0c48", "#210c4a", "#230c4c", "#240c4f", "#260c51", "#280b53", "#290b55", "#290b55",
                "#2b0b57", "#2d0b59", "#2f0a5b", "#310a5c", "#320a5e", "#340a5f", "#360961", "#380962",
                "#390963", "#3b0964", "#3d0965", "#3e0966", "#400a67", "#420a68", "#440a68", "#450a69",
                "#470b6a", "#490b6a", "#4a0c6b", "#4c0c6b", "#4d0d6c", "#4f0d6c", "#510e6c", "#520e6d",
                "#520e6d", "#540f6d", "#550f6d", "#57106e", "#59106e", "#5a116e", "#5c126e", "#5d126e",
                "#5f136e", "#61136e", "#62146e", "#64156e", "#65156e", "#67166e", "#69166e", "#6a176e",
                "#6c186e", "#6d186e", "#6f196e", "#71196e", "#721a6e", "#741a6e", "#751b6e", "#771c6d",
                "#781c6d", "#7a1d6d", "#7a1d6d", "#7c1d6d", "#7d1e6d", "#7f1e6c", "#801f6c", "#82206c",
                "#84206b", "#85216b", "#87216b", "#88226a", "#8a226a", "#8c2369", "#8d2369", "#8f2469",
                "#902568", "#922568", "#932667", "#952667", "#972766", "#982766", "#9a2865", "#9b2964",
                "#9d2964", "#9f2a63", "#a02a63", "#a02a63", "#a22b62", "#a32c61", "#a52c60", "#a62d60",
                "#a82e5f", "#a92e5e", "#ab2f5e", "#ad305d", "#ae305c", "#b0315b", "#b1325a", "#b3325a",
                "#b43359", "#b63458", "#b73557", "#b93556", "#ba3655", "#bc3754", "#bd3853", "#bf3952",
                "#c03a51", "#c13a50", "#c33b4f", "#c43c4e", "#c63d4d", "#c63d4d", "#c73e4c", "#c83f4b",
                "#ca404a", "#cb4149", "#cc4248", "#ce4347", "#cf4446", "#d04545", "#d24644", "#d34743",
                "#d44842", "#d54a41", "#d74b3f", "#d84c3e", "#d94d3d", "#da4e3c", "#db503b", "#dd513a",
                "#de5238", "#df5337", "#e05536", "#e15635", "#e25734", "#e35933", "#e35933", "#e45a31",
                "#e55c30", "#e65d2f", "#e75e2e", "#e8602d", "#e9612b", "#ea632a", "#eb6429", "#eb6628",
                "#ec6726", "#ed6925", "#ee6a24", "#ef6c23", "#ef6e21", "#f06f20", "#f1711f", "#f1731d",
                "#f2741c", "#f3761b", "#f37819", "#f47918", "#f57b17", "#f57d15", "#f67e14", "#f68013",
                "#f68013", "#f78212", "#f78410", "#f8850f", "#f8870e", "#f8890c", "#f98b0b", "#f98c0a",
                "#f98e09", "#fa9008", "#fa9207", "#fa9407", "#fb9606", "#fb9706", "#fb9906", "#fb9b06",
                "#fb9d07", "#fc9f07", "#fca108", "#fca309", "#fca50a", "#fca60c", "#fca80d", "#fcaa0f",
                "#fcac11", "#fcac11", "#fcae12", "#fcb014", "#fcb216", "#fcb418", "#fbb61a", "#fbb81d",
                "#fbba1f", "#fbbc21", "#fbbe23", "#fac026", "#fac228", "#fac42a", "#fac62d", "#f9c72f",
                "#f9c932", "#f9cb35", "#f8cd37", "#f8cf3a", "#f7d13d", "#f7d340", "#f6d543", "#f6d746",
                "#f5d949", "#f5db4c", "#f4dd4f", "#f4dd4f", "#f4df53", "#f4e156", "#f3e35a", "#f3e55d",
                "#f2e661", "#f2e865", "#f2ea69", "#f1ec6d", "#f1ed71", "#f1ef75", "#f1f179", "#f2f27d"
        ));

        public static ColorMapContainer Magma = ColorMapContainer.FromStringList(new List<string>(
                "#000004", "#010005", "#010106", "#010108", "#020109", "#02020b", "#02020d", "#03030f",
                "#030312", "#040414", "#050416", "#060518", "#06051a", "#06051a", "#07061c", "#08071e",
                "#090720", "#0a0822", "#0b0924", "#0c0926", "#0d0a29", "#0e0b2b", "#100b2d", "#110c2f",
                "#120d31", "#130d34", "#140e36", "#150e38", "#160f3b", "#180f3d", "#19103f", "#1a1042",
                "#1c1044", "#1d1147", "#1e1149", "#20114b", "#21114e", "#221150", "#241253", "#241253",
                "#251255", "#271258", "#29115a", "#2a115c", "#2c115f", "#2d1161", "#2f1163", "#311165",
                "#331067", "#341069", "#36106b", "#38106c", "#390f6e", "#3b0f70", "#3d0f71", "#3f0f72",
                "#400f74", "#420f75", "#440f76", "#451077", "#471078", "#491078", "#4a1079", "#4c117a",
                "#4c117a", "#4e117b", "#4f127b", "#51127c", "#52137c", "#54137d", "#56147d", "#57157e",
                "#59157e", "#5a167e", "#5c167f", "#5d177f", "#5f187f", "#601880", "#621980", "#641a80",
                "#651a80", "#671b80", "#681c81", "#6a1c81", "#6b1d81", "#6d1d81", "#6e1e81", "#701f81",
                "#721f81", "#732081", "#732081", "#752181", "#762181", "#782281", "#792282", "#7b2382",
                "#7c2382", "#7e2482", "#802582", "#812581", "#832681", "#842681", "#862781", "#882781",
                "#892881", "#8b2981", "#8c2981", "#8e2a81", "#902a81", "#912b81", "#932b80", "#942c80",
                "#962c80", "#982d80", "#992d80", "#992d80", "#9b2e7f", "#9c2e7f", "#9e2f7f", "#a02f7f",
                "#a1307e", "#a3307e", "#a5317e", "#a6317d", "#a8327d", "#aa337d", "#ab337c", "#ad347c",
                "#ae347b", "#b0357b", "#b2357b", "#b3367a", "#b5367a", "#b73779", "#b83779", "#ba3878",
                "#bc3978", "#bd3977", "#bf3a77", "#c03a76", "#c23b75", "#c23b75", "#c43c75", "#c53c74",
                "#c73d73", "#c83e73", "#ca3e72", "#cc3f71", "#cd4071", "#cf4070", "#d0416f", "#d2426f",
                "#d3436e", "#d5446d", "#d6456c", "#d8456c", "#d9466b", "#db476a", "#dc4869", "#de4968",
                "#df4a68", "#e04c67", "#e24d66", "#e34e65", "#e44f64", "#e55064", "#e55064", "#e75263",
                "#e85362", "#e95462", "#ea5661", "#eb5760", "#ec5860", "#ed5a5f", "#ee5b5e", "#ef5d5e",
                "#f05f5e", "#f1605d", "#f2625d", "#f2645c", "#f3655c", "#f4675c", "#f4695c", "#f56b5c",
                "#f66c5c", "#f66e5c", "#f7705c", "#f7725c", "#f8745c", "#f8765c", "#f9785d", "#f9795d",
                "#f9795d", "#f97b5d", "#fa7d5e", "#fa7f5e", "#fa815f", "#fb835f", "#fb8560", "#fb8761",
                "#fc8961", "#fc8a62", "#fc8c63", "#fc8e64", "#fc9065", "#fd9266", "#fd9467", "#fd9668",
                "#fd9869", "#fd9a6a", "#fd9b6b", "#fe9d6c", "#fe9f6d", "#fea16e", "#fea36f", "#fea571",
                "#fea772", "#fea772", "#fea973", "#feaa74", "#feac76", "#feae77", "#feb078", "#feb27a",
                "#feb47b", "#feb67c", "#feb77e", "#feb97f", "#febb81", "#febd82", "#febf84", "#fec185",
                "#fec287", "#fec488", "#fec68a", "#fec88c", "#feca8d", "#fecc8f", "#fecd90", "#fecf92",
                "#fed194", "#fed395", "#fed597", "#fed597", "#fed799", "#fed89a", "#fdda9c", "#fddc9e",
                "#fddea0", "#fde0a1", "#fde2a3", "#fde3a5", "#fde5a7", "#fde7a9", "#fde9aa", "#fdebac"
        ));

        public static ColorMapContainer Cividis = ColorMapContainer.FromStringList(new List<string>(
                "#00224e", "#00234f", "#002451", "#002553", "#002554", "#002656", "#002758", "#002859",
                "#00285b", "#00295d", "#002a5f", "#002a61", "#002b62", "#002b62", "#002c64", "#002c66",
                "#002d68", "#002e6a", "#002e6c", "#002f6d", "#00306f", "#003070", "#003170", "#003171",
                "#013271", "#053371", "#083370", "#0c3470", "#0f3570", "#123570", "#143670", "#163770",
                "#18376f", "#1a386f", "#1c396f", "#1e3a6f", "#203a6f", "#213b6e", "#233c6e", "#233c6e",
                "#243c6e", "#263d6e", "#273e6e", "#293f6e", "#2a3f6d", "#2b406d", "#2d416d", "#2e416d",
                "#2f426d", "#31436d", "#32436d", "#33446d", "#34456c", "#35456c", "#36466c", "#38476c",
                "#39486c", "#3a486c", "#3b496c", "#3c4a6c", "#3d4a6c", "#3e4b6c", "#3f4c6c", "#404c6c",
                "#404c6c", "#414d6c", "#424e6c", "#434e6c", "#444f6c", "#45506c", "#46516c", "#47516c",
                "#48526c", "#49536c", "#4a536c", "#4b546c", "#4c556c", "#4d556c", "#4e566c", "#4f576c",
                "#50576c", "#51586d", "#52596d", "#535a6d", "#545a6d", "#555b6d", "#555c6d", "#565c6d",
                "#575d6d", "#585e6d", "#585e6d", "#595e6e", "#5a5f6e", "#5b606e", "#5c616e", "#5d616e",
                "#5e626e", "#5e636f", "#5f636f", "#60646f", "#61656f", "#62656f", "#636670", "#646770",
                "#656870", "#656870", "#666970", "#676a71", "#686a71", "#696b71", "#6a6c71", "#6b6d72",
                "#6c6d72", "#6c6e72", "#6d6f72", "#6d6f72", "#6e6f73", "#6f7073", "#707173", "#717274",
                "#727274", "#727374", "#737475", "#747475", "#757575", "#767676", "#777776", "#777777",
                "#787877", "#797977", "#7a7a78", "#7b7a78", "#7c7b78", "#7d7c78", "#7e7c78", "#7e7d78",
                "#7f7e78", "#807f78", "#817f78", "#828079", "#838179", "#838179", "#848279", "#858279",
                "#868379", "#878478", "#888578", "#898578", "#8a8678", "#8b8778", "#8c8878", "#8d8878",
                "#8e8978", "#8f8a78", "#908b78", "#918b78", "#928c78", "#928d78", "#938e78", "#948e77",
                "#958f77", "#969077", "#979177", "#989277", "#999277", "#9a9376", "#9a9376", "#9b9476",
                "#9c9576", "#9d9576", "#9e9676", "#9f9775", "#a09875", "#a19975", "#a29975", "#a39a74",
                "#a49b74", "#a59c74", "#a69c74", "#a79d73", "#a89e73", "#a99f73", "#aaa073", "#aba072",
                "#aca172", "#ada272", "#aea371", "#afa471", "#b0a571", "#b1a570", "#b3a670", "#b4a76f",
                "#b4a76f", "#b5a86f", "#b6a96f", "#b7a96e", "#b8aa6e", "#b9ab6d", "#baac6d", "#bbad6d",
                "#bcae6c", "#bdae6c", "#beaf6b", "#bfb06b", "#c0b16a", "#c1b26a", "#c2b369", "#c3b369",
                "#c4b468", "#c5b568", "#c6b667", "#c7b767", "#c8b866", "#c9b965", "#cbb965", "#ccba64",
                "#cdbb63", "#cdbb63", "#cebc63", "#cfbd62", "#d0be62", "#d1bf61", "#d2c060", "#d3c05f",
                "#d4c15f", "#d5c25e", "#d6c35d", "#d7c45c", "#d9c55c", "#dac65b", "#dbc75a", "#dcc859",
                "#ddc858", "#dec958", "#dfca57", "#e0cb56", "#e1cc55", "#e2cd54", "#e4ce53", "#e5cf52",
                "#e6d051", "#e7d150", "#e8d24f", "#e8d24f", "#e9d34e", "#ead34c", "#ebd44b", "#edd54a",
                "#eed649", "#efd748", "#f0d846", "#f1d945", "#f2da44", "#f3db42", "#f5dc41", "#f6dd3f"
        ));

        public static ColorMapContainer Greys = ColorMapContainer.FromStringList(new List<string>(
                "#ffffff", "#ffffff", "#fefefe", "#fefefe", "#fdfdfd", "#fdfdfd", "#fcfcfc", "#fcfcfc",
                "#fbfbfb", "#fbfbfb", "#fafafa", "#fafafa", "#f9f9f9", "#f9f9f9", "#f9f9f9", "#f8f8f8",
                "#f8f8f8", "#f7f7f7", "#f7f7f7", "#f7f7f7", "#f6f6f6", "#f6f6f6", "#f5f5f5", "#f5f5f5",
                "#f4f4f4", "#f4f4f4", "#f3f3f3", "#f3f3f3", "#f2f2f2", "#f2f2f2", "#f1f1f1", "#f1f1f1",
                "#f0f0f0", "#f0f0f0", "#efefef", "#eeeeee", "#eeeeee", "#ededed", "#ececec", "#ececec",
                "#ececec", "#ebebeb", "#eaeaea", "#e9e9e9", "#e9e9e9", "#e8e8e8", "#e7e7e7", "#e7e7e7",
                "#e6e6e6", "#e5e5e5", "#e4e4e4", "#e4e4e4", "#e3e3e3", "#e2e2e2", "#e1e1e1", "#e1e1e1",
                "#e0e0e0", "#dfdfdf", "#dfdfdf", "#dedede", "#dddddd", "#dcdcdc", "#dcdcdc", "#dbdbdb",
                "#dbdbdb", "#dadada", "#dadada", "#d9d9d9", "#d8d8d8", "#d7d7d7", "#d6d6d6", "#d5d5d5",
                "#d4d4d4", "#d4d4d4", "#d3d3d3", "#d2d2d2", "#d1d1d1", "#d0d0d0", "#cfcfcf", "#cecece",
                "#cdcdcd", "#cccccc", "#cccccc", "#cbcbcb", "#cacaca", "#c9c9c9", "#c8c8c8", "#c7c7c7",
                "#c6c6c6", "#c5c5c5", "#c5c5c5", "#c5c5c5", "#c4c4c4", "#c3c3c3", "#c2c2c2", "#c1c1c1",
                "#c0c0c0", "#bfbfbf", "#bebebe", "#bebebe", "#bdbdbd", "#bbbbbb", "#bababa", "#b9b9b9",
                "#b8b8b8", "#b6b6b6", "#b5b5b5", "#b4b4b4", "#b3b3b3", "#b2b2b2", "#b0b0b0", "#afafaf",
                "#aeaeae", "#adadad", "#ababab", "#ababab", "#aaaaaa", "#a9a9a9", "#a8a8a8", "#a7a7a7",
                "#a5a5a5", "#a4a4a4", "#a3a3a3", "#a2a2a2", "#a0a0a0", "#9f9f9f", "#9e9e9e", "#9d9d9d",
                "#9c9c9c", "#9a9a9a", "#999999", "#989898", "#979797", "#959595", "#949494", "#939393",
                "#929292", "#919191", "#909090", "#8f8f8f", "#8e8e8e", "#8e8e8e", "#8d8d8d", "#8c8c8c",
                "#8a8a8a", "#898989", "#888888", "#878787", "#868686", "#858585", "#848484", "#838383",
                "#828282", "#818181", "#7f7f7f", "#7e7e7e", "#7d7d7d", "#7c7c7c", "#7b7b7b", "#7a7a7a",
                "#797979", "#787878", "#777777", "#767676", "#757575", "#737373", "#737373", "#727272",
                "#717171", "#707070", "#6f6f6f", "#6e6e6e", "#6d6d6d", "#6c6c6c", "#6b6b6b", "#6a6a6a",
                "#696969", "#686868", "#676767", "#666666", "#656565", "#646464", "#636363", "#626262",
                "#616161", "#606060", "#5f5f5f", "#5e5e5e", "#5d5d5d", "#5c5c5c", "#5b5b5b", "#5a5a5a",
                "#5a5a5a", "#585858", "#575757", "#565656", "#555555", "#545454", "#535353", "#525252",
                "#515151", "#505050", "#4e4e4e", "#4d4d4d", "#4b4b4b", "#4a4a4a", "#484848", "#474747",
                "#464646", "#444444", "#434343", "#414141", "#404040", "#3f3f3f", "#3d3d3d", "#3c3c3c",
                "#3a3a3a", "#3a3a3a", "#393939", "#383838", "#363636", "#353535", "#333333", "#323232",
                "#303030", "#2f2f2f", "#2e2e2e", "#2c2c2c", "#2b2b2b", "#292929", "#282828", "#272727",
                "#252525", "#242424", "#232323", "#222222", "#212121", "#1f1f1f", "#1e1e1e", "#1d1d1d",
                "#1c1c1c", "#1b1b1b", "#1a1a1a", "#1a1a1a", "#181818", "#171717", "#161616", "#151515",
                "#141414", "#131313", "#111111", "#101010", "#0f0f0f", "#0e0e0e", "#0d0d0d", "#0c0c0c"
        ));

        public static ColorMapContainer Purples = ColorMapContainer.FromStringList(new List<string>(
                "#fcfbfd", "#fcfbfd", "#fbfafc", "#fbfafc", "#faf9fc", "#faf9fc", "#faf8fb", "#f9f8fb",
                "#f9f7fb", "#f8f7fb", "#f8f7fa", "#f8f6fa", "#f7f6fa", "#f7f6fa", "#f7f5fa", "#f6f5f9",
                "#f6f4f9", "#f5f4f9", "#f5f4f9", "#f5f3f8", "#f4f3f8", "#f4f2f8", "#f3f2f8", "#f3f1f7",
                "#f3f1f7", "#f2f0f7", "#f2f0f7", "#f1f0f6", "#f1eff6", "#f1eff6", "#f0eef6", "#f0eef5",
                "#efedf5", "#efedf5", "#eeecf5", "#eeecf4", "#edebf4", "#ecebf4", "#eceaf3", "#eceaf3",
                "#ebe9f3", "#eae9f3", "#eae8f2", "#e9e8f2", "#e8e7f2", "#e8e6f2", "#e7e6f1", "#e6e5f1",
                "#e6e5f1", "#e5e4f0", "#e4e3f0", "#e4e3f0", "#e3e2ef", "#e2e2ef", "#e2e1ef", "#e1e0ee",
                "#e0e0ee", "#e0dfee", "#dfdfed", "#dedeed", "#dedded", "#ddddec", "#dcdcec", "#dcdcec",
                "#dcdcec", "#dbdbec", "#dadaeb", "#dadaeb", "#d9d9ea", "#d8d8ea", "#d7d7e9", "#d6d6e9",
                "#d5d5e9", "#d4d4e8", "#d3d3e8", "#d2d2e7", "#d1d2e7", "#d0d1e6", "#cfd0e6", "#cecfe5",
                "#cecee5", "#cdcde4", "#cccce4", "#cbcbe3", "#cacae3", "#c9c9e2", "#c8c8e2", "#c7c8e1",
                "#c6c7e1", "#c5c6e1", "#c5c6e1", "#c4c5e0", "#c3c4e0", "#c2c3df", "#c1c2df", "#c0c1de",
                "#bfc0de", "#bebfdd", "#bebedd", "#bdbedc", "#bcbddc", "#bbbbdb", "#babadb", "#b9b9da",
                "#b8b8d9", "#b7b7d9", "#b6b6d8", "#b5b5d7", "#b4b4d7", "#b3b3d6", "#b2b2d5", "#b1b1d5",
                "#b0afd4", "#afaed4", "#aeadd3", "#aeadd3", "#aeacd2", "#adabd2", "#acaad1", "#aba9d0",
                "#aaa8d0", "#a9a7cf", "#a8a6cf", "#a7a4ce", "#a6a3cd", "#a5a2cd", "#a4a1cc", "#a3a0cb",
                "#a29fcb", "#a19eca", "#a09dca", "#9f9cc9", "#9e9bc8", "#9e9ac8", "#9d99c7", "#9c98c7",
                "#9b97c6", "#9a96c6", "#9995c6", "#9894c5", "#9793c5", "#9793c5", "#9692c4", "#9591c4",
                "#9490c3", "#9390c3", "#928fc3", "#918ec2", "#908dc2", "#8f8cc1", "#8e8bc1", "#8e8ac0",
                "#8d89c0", "#8c88bf", "#8b87bf", "#8a86bf", "#8986be", "#8885be", "#8784bd", "#8683bd",
                "#8582bc", "#8481bc", "#8380bb", "#827fbb", "#817ebb", "#807dba", "#807dba", "#807cba",
                "#7f7bb9", "#7e79b8", "#7d78b7", "#7d77b7", "#7c75b6", "#7b74b5", "#7b72b4", "#7a71b4",
                "#7970b3", "#796eb2", "#786db2", "#776cb1", "#776ab0", "#7669af", "#7567af", "#7566ae",
                "#7465ad", "#7363ad", "#7262ac", "#7261ab", "#715faa", "#705eaa", "#705ca9", "#6f5ba8",
                "#6f5ba8", "#6e5aa8", "#6e58a7", "#6d57a6", "#6c55a5", "#6c54a5", "#6b53a4", "#6a51a3",
                "#6950a3", "#694fa2", "#684da1", "#674ca1", "#674ba0", "#66499f", "#65489f", "#65479e",
                "#64459e", "#63449d", "#63439c", "#62429c", "#61409b", "#613f9a", "#603e9a", "#5f3c99",
                "#5e3b98", "#5e3b98", "#5e3a98", "#5d3897", "#5c3797", "#5c3696", "#5b3495", "#5a3395",
                "#5a3294", "#593093", "#582f93", "#582e92", "#572c92", "#562b91", "#552a90", "#552890",
                "#54278f", "#53268f", "#53258e", "#52238d", "#51228d", "#51218c", "#50208c", "#4f1f8b",
                "#4f1d8b", "#4e1c8a", "#4d1b89", "#4d1b89", "#4d1a89", "#4c1888", "#4c1788", "#4b1687",
                "#4a1587", "#4a1486", "#491285", "#481185", "#481084", "#470f84", "#460d83", "#460c83"
        ));

        public static ColorMapContainer Blues = ColorMapContainer.FromStringList(new List<string>(
                "#f7fbff", "#f6faff", "#f5fafe", "#f5f9fe", "#f4f9fe", "#f3f8fe", "#f2f8fd", "#f2f7fd",
                "#f1f7fd", "#f0f6fd", "#eff6fc", "#eef5fc", "#eef5fc", "#eef5fc", "#edf4fc", "#ecf4fb",
                "#ebf3fb", "#eaf3fb", "#eaf2fb", "#e9f2fa", "#e8f1fa", "#e7f1fa", "#e7f0fa", "#e6f0f9",
                "#e5eff9", "#e4eff9", "#e3eef9", "#e3eef8", "#e2edf8", "#e1edf8", "#e0ecf8", "#dfecf7",
                "#dfebf7", "#deebf7", "#ddeaf7", "#dceaf6", "#dce9f6", "#dbe9f6", "#dae8f6", "#dae8f6",
                "#d9e8f5", "#d9e7f5", "#d8e7f5", "#d7e6f5", "#d6e6f4", "#d6e5f4", "#d5e5f4", "#d4e4f4",
                "#d3e4f3", "#d3e3f3", "#d2e3f3", "#d1e2f3", "#d0e2f2", "#d0e1f2", "#cfe1f2", "#cee0f2",
                "#cde0f1", "#cddff1", "#ccdff1", "#cbdef1", "#cadef0", "#caddf0", "#c9ddf0", "#c8dcf0",
                "#c8dcf0", "#c7dcef", "#c7dbef", "#c6dbef", "#c4daee", "#c3daee", "#c2d9ee", "#c1d9ed",
                "#bfd8ed", "#bed8ec", "#bdd7ec", "#bcd7eb", "#bad6eb", "#b9d6ea", "#b8d5ea", "#b7d4ea",
                "#b5d4e9", "#b4d3e9", "#b3d3e8", "#b2d2e8", "#b0d2e7", "#afd1e7", "#aed1e7", "#add0e6",
                "#abd0e6", "#aacfe5", "#aacfe5", "#a9cfe5", "#a8cee4", "#a6cee4", "#a5cde3", "#a4cce3",
                "#a3cce3", "#a1cbe2", "#a0cbe2", "#9fcae1", "#9dcae1", "#9cc9e1", "#9ac8e0", "#99c7e0",
                "#97c6df", "#95c5df", "#94c4df", "#92c4de", "#91c3de", "#8fc2de", "#8dc1dd", "#8cc0dd",
                "#8abfdd", "#89bedc", "#87bddc", "#87bddc", "#85bcdc", "#84bcdb", "#82bbdb", "#81badb",
                "#7fb9da", "#7db8da", "#7cb7da", "#7ab6d9", "#79b5d9", "#77b5d9", "#75b4d8", "#74b3d8",
                "#72b2d8", "#71b1d7", "#6fb0d7", "#6dafd7", "#6caed6", "#6aaed6", "#69add5", "#68acd5",
                "#66abd4", "#65aad4", "#64a9d3", "#63a8d3", "#61a7d2", "#61a7d2", "#60a7d2", "#5fa6d1",
                "#5da5d1", "#5ca4d0", "#5ba3d0", "#5aa2cf", "#58a1cf", "#57a0ce", "#56a0ce", "#549fcd",
                "#539ecd", "#529dcc", "#519ccc", "#4f9bcb", "#4e9acb", "#4d99ca", "#4b98ca", "#4a98c9",
                "#4997c9", "#4896c8", "#4695c8", "#4594c7", "#4493c7", "#4292c6", "#4292c6", "#4191c6",
                "#4090c5", "#3f8fc5", "#3e8ec4", "#3d8dc4", "#3c8cc3", "#3b8bc2", "#3a8ac2", "#3989c1",
                "#3888c1", "#3787c0", "#3686c0", "#3585bf", "#3484bf", "#3383be", "#3282be", "#3181bd",
                "#3080bd", "#2f7fbc", "#2e7ebc", "#2d7dbb", "#2c7cba", "#2b7bba", "#2a7ab9", "#2979b9",
                "#2979b9", "#2777b8", "#2676b8", "#2575b7", "#2474b7", "#2373b6", "#2272b6", "#2171b5",
                "#2070b4", "#206fb4", "#1f6eb3", "#1e6db2", "#1d6cb1", "#1c6bb0", "#1c6ab0", "#1b69af",
                "#1a68ae", "#1967ad", "#1966ad", "#1865ac", "#1764ab", "#1663aa", "#1562a9", "#1561a9",
                "#1460a8", "#1460a8", "#135fa7", "#125ea6", "#125da6", "#115ca5", "#105ba4", "#0f5aa3",
                "#0e59a2", "#0e58a2", "#0d57a1", "#0c56a0", "#0b559f", "#0a549e", "#0a539e", "#09529d",
                "#08519c", "#08509b", "#084f99", "#084e98", "#084d96", "#084c95", "#084b93", "#084a91",
                "#084990", "#08488e", "#08478d", "#08478d", "#08468b", "#08458a", "#084488", "#084387",
                "#084285", "#084184", "#084082", "#083e81", "#083d7f", "#083c7d", "#083b7c", "#083a7a"
        ));

        public static ColorMapContainer Greens = ColorMapContainer.FromStringList(new List<string>(
                "#f7fcf5", "#f6fcf4", "#f6fcf4", "#f5fbf3", "#f5fbf2", "#f4fbf2", "#f4fbf1", "#f3faf0",
                "#f2faf0", "#f2faef", "#f1faee", "#f1faee", "#f0f9ed", "#f0f9ed", "#f0f9ec", "#eff9ec",
                "#eff9eb", "#eef8ea", "#edf8ea", "#edf8e9", "#ecf8e8", "#ecf8e8", "#ebf7e7", "#ebf7e7",
                "#eaf7e6", "#e9f7e5", "#e9f7e5", "#e8f6e4", "#e8f6e3", "#e7f6e3", "#e7f6e2", "#e6f5e1",
                "#e5f5e1", "#e5f5e0", "#e4f5df", "#e3f4de", "#e2f4dd", "#e1f3dc", "#e0f3db", "#e0f3db",
                "#dff3da", "#def2d9", "#ddf2d8", "#dcf2d7", "#dbf1d6", "#dbf1d5", "#daf0d4", "#d9f0d3",
                "#d8f0d2", "#d7efd1", "#d6efd0", "#d5efcf", "#d4eece", "#d3eecd", "#d2edcc", "#d1edcb",
                "#d0edca", "#cfecc9", "#ceecc8", "#cdecc7", "#ccebc6", "#cbebc5", "#cbeac4", "#caeac3",
                "#caeac3", "#c9eac2", "#c8e9c1", "#c7e9c0", "#c6e8bf", "#c4e8bd", "#c3e7bc", "#c2e7bb",
                "#c1e6ba", "#c0e6b9", "#bee5b8", "#bde5b6", "#bce4b5", "#bbe4b4", "#bae3b3", "#b8e3b2",
                "#b7e2b1", "#b6e2af", "#b5e1ae", "#b4e1ad", "#b2e0ac", "#b1e0ab", "#b0dfaa", "#afdfa8",
                "#aedea7", "#acdea6", "#acdea6", "#abdda5", "#aadda4", "#a9dca3", "#a8dca2", "#a7dba0",
                "#a5db9f", "#a4da9e", "#a3da9d", "#a2d99c", "#a0d99b", "#9fd899", "#9ed798", "#9cd797",
                "#9bd696", "#99d595", "#98d594", "#97d492", "#95d391", "#94d390", "#92d28f", "#91d28e",
                "#90d18d", "#8ed08b", "#8dd08a", "#8dd08a", "#8bcf89", "#8ace88", "#88ce87", "#87cd86",
                "#86cc85", "#84cc83", "#83cb82", "#81ca81", "#80ca80", "#7fc97f", "#7dc87e", "#7cc87c",
                "#7ac77b", "#79c67a", "#78c679", "#76c578", "#75c477", "#73c476", "#72c375", "#70c274",
                "#6ec173", "#6dc072", "#6bc072", "#6abf71", "#68be70", "#68be70", "#66bd6f", "#65bd6f",
                "#63bc6e", "#62bb6d", "#60ba6c", "#5eb96b", "#5db96b", "#5bb86a", "#5ab769", "#58b668",
                "#56b567", "#55b567", "#53b466", "#52b365", "#50b264", "#4eb264", "#4db163", "#4bb062",
                "#4aaf61", "#48ae60", "#46ae60", "#45ad5f", "#43ac5e", "#42ab5d", "#42ab5d", "#40aa5d",
                "#3fa95c", "#3fa85b", "#3ea75a", "#3da65a", "#3ca559", "#3ba458", "#3aa357", "#39a257",
                "#38a156", "#37a055", "#369f54", "#359e53", "#349d53", "#339c52", "#329b51", "#319a50",
                "#309950", "#2f984f", "#2f974e", "#2e964d", "#2d954d", "#2c944c", "#2b934b", "#2a924a",
                "#2a924a", "#29914a", "#289049", "#278f48", "#268e47", "#258d47", "#248c46", "#238b45",
                "#228a44", "#218944", "#208843", "#1f8742", "#1e8741", "#1d8640", "#1c8540", "#1a843f",
                "#19833e", "#18823d", "#17813d", "#16803c", "#157f3b", "#147e3a", "#137d39", "#127c39",
                "#117b38", "#117b38", "#107a37", "#0e7936", "#0d7836", "#0c7735", "#0b7734", "#0a7633",
                "#097532", "#087432", "#077331", "#067230", "#05712f", "#03702e", "#026f2e", "#016e2d",
                "#006d2c", "#006c2c", "#006b2b", "#00692a", "#00682a", "#006729", "#006529", "#006428",
                "#006328", "#006227", "#006027", "#006027", "#005f26", "#005e26", "#005c25", "#005b25",
                "#005a24", "#005924", "#005723", "#005622", "#005522", "#005321", "#005221", "#005120"
        ));

        public static ColorMapContainer Oranges = ColorMapContainer.FromStringList(new List<string>(
                "#fff5eb", "#fff5ea", "#fff4e9", "#fff4e8", "#fff3e7", "#fff3e6", "#fff2e6", "#fff2e5",
                "#fff1e4", "#fff1e3", "#fff0e2", "#fff0e1", "#ffefe0", "#ffefe0", "#ffefdf", "#ffeede",
                "#ffeedd", "#feeddc", "#feeddc", "#feeddb", "#feecda", "#feecd9", "#feebd8", "#feebd7",
                "#feead6", "#feead5", "#fee9d4", "#fee9d3", "#fee8d2", "#fee8d2", "#fee7d1", "#fee7d0",
                "#fee6cf", "#fee6ce", "#fee5cc", "#fee5cb", "#fee4ca", "#fee3c8", "#fee2c7", "#fee2c7",
                "#fee2c6", "#fee1c4", "#fee0c3", "#fee0c1", "#fedfc0", "#fedebf", "#fedebd", "#feddbc",
                "#fedcbb", "#fedcb9", "#fddbb8", "#fddab6", "#fdd9b5", "#fdd9b4", "#fdd8b2", "#fdd7b1",
                "#fdd7af", "#fdd6ae", "#fdd5ad", "#fdd5ab", "#fdd4aa", "#fdd3a9", "#fdd3a7", "#fdd2a6",
                "#fdd2a6", "#fdd1a4", "#fdd1a3", "#fdd0a2", "#fdcfa0", "#fdce9e", "#fdcd9c", "#fdcb9b",
                "#fdca99", "#fdc997", "#fdc895", "#fdc794", "#fdc692", "#fdc590", "#fdc48f", "#fdc38d",
                "#fdc28b", "#fdc189", "#fdc088", "#fdbf86", "#fdbe84", "#fdbd83", "#fdbb81", "#fdba7f",
                "#fdb97d", "#fdb87c", "#fdb87c", "#fdb77a", "#fdb678", "#fdb576", "#fdb475", "#fdb373",
                "#fdb271", "#fdb170", "#fdb06e", "#fdaf6c", "#fdae6a", "#fdad69", "#fdac67", "#fdab66",
                "#fda965", "#fda863", "#fda762", "#fda660", "#fda55f", "#fda45d", "#fda35c", "#fda25a",
                "#fda159", "#fda057", "#fd9f56", "#fd9f56", "#fd9e54", "#fd9d53", "#fd9c51", "#fd9b50",
                "#fd9a4e", "#fd994d", "#fd984b", "#fd974a", "#fd9649", "#fd9547", "#fd9446", "#fd9344",
                "#fd9243", "#fd9141", "#fd9040", "#fd8f3e", "#fd8e3d", "#fd8c3b", "#fc8b3a", "#fc8a39",
                "#fc8937", "#fb8836", "#fb8735", "#fb8634", "#fa8532", "#fa8532", "#fa8331", "#f98230",
                "#f9812e", "#f9802d", "#f87f2c", "#f87e2b", "#f87d29", "#f77b28", "#f77a27", "#f67925",
                "#f67824", "#f67723", "#f57622", "#f57520", "#f5741f", "#f4721e", "#f4711c", "#f3701b",
                "#f36f1a", "#f36e19", "#f26d17", "#f26c16", "#f26b15", "#f16913", "#f16913", "#f16813",
                "#f06712", "#ef6612", "#ee6511", "#ee6410", "#ed6310", "#ec620f", "#eb610f", "#eb600e",
                "#ea5f0e", "#e95e0d", "#e85d0c", "#e75c0c", "#e75b0b", "#e65a0b", "#e5590a", "#e4580a",
                "#e45709", "#e35608", "#e25508", "#e15407", "#e15307", "#e05206", "#df5106", "#de5005",
                "#de5005", "#de4e05", "#dd4d04", "#dc4c03", "#db4b03", "#db4a02", "#da4902", "#d94801",
                "#d84801", "#d64701", "#d54601", "#d34601", "#d14501", "#d04501", "#ce4401", "#cd4401",
                "#cb4302", "#c94202", "#c84202", "#c64102", "#c54102", "#c34002", "#c14002", "#c03f02",
                "#be3f02", "#be3f02", "#bd3e02", "#bb3d02", "#b93d02", "#b83c02", "#b63c02", "#b53b02",
                "#b33b02", "#b13a03", "#b03903", "#ae3903", "#ad3803", "#ab3803", "#a93703", "#a83703",
                "#a63603", "#a53603", "#a43503", "#a23503", "#a13403", "#a03403", "#9f3303", "#9e3303",
                "#9c3203", "#9b3203", "#9a3103", "#9a3103", "#993103", "#973003", "#963003", "#952f03",
                "#942f03", "#932f03", "#912e04", "#902e04", "#8f2d04", "#8e2d04", "#8c2c04", "#8b2c04"
        ));

        public static ColorMapContainer Reds = ColorMapContainer.FromStringList(new List<string>(
                "#fff5f0", "#fff4ef", "#fff4ee", "#fff3ed", "#fff2ec", "#fff2eb", "#fff1ea", "#fff0e9",
                "#fff0e8", "#ffefe8", "#ffeee7", "#ffeee6", "#ffede5", "#ffede5", "#ffece4", "#ffece3",
                "#ffebe2", "#feeae1", "#feeae0", "#fee9df", "#fee8de", "#fee8dd", "#fee7dc", "#fee7db",
                "#fee6da", "#fee5d9", "#fee5d8", "#fee4d8", "#fee3d7", "#fee3d6", "#fee2d5", "#fee1d4",
                "#fee1d3", "#fee0d2", "#fedfd0", "#fedecf", "#fedccd", "#fedbcc", "#fedaca", "#fedaca",
                "#fed9c9", "#fed8c7", "#fdd7c6", "#fdd5c4", "#fdd4c2", "#fdd3c1", "#fdd2bf", "#fdd1be",
                "#fdd0bc", "#fdcebb", "#fdcdb9", "#fdccb8", "#fdcbb6", "#fdcab5", "#fdc9b3", "#fdc7b2",
                "#fdc6b0", "#fdc5ae", "#fcc4ad", "#fcc3ab", "#fcc2aa", "#fcc1a8", "#fcbfa7", "#fcbea5",
                "#fcbea5", "#fcbda4", "#fcbca2", "#fcbba1", "#fcb99f", "#fcb89e", "#fcb79c", "#fcb69b",
                "#fcb499", "#fcb398", "#fcb296", "#fcb095", "#fcaf93", "#fcae92", "#fcad90", "#fcab8f",
                "#fcaa8d", "#fca98c", "#fca78b", "#fca689", "#fca588", "#fca486", "#fca285", "#fca183",
                "#fca082", "#fc9e80", "#fc9e80", "#fc9d7f", "#fc9c7d", "#fc9b7c", "#fc997a", "#fc9879",
                "#fc9777", "#fc9576", "#fc9474", "#fc9373", "#fc9272", "#fc9070", "#fc8f6f", "#fc8e6e",
                "#fc8d6d", "#fc8b6b", "#fc8a6a", "#fc8969", "#fc8767", "#fc8666", "#fc8565", "#fc8464",
                "#fc8262", "#fc8161", "#fc8060", "#fc8060", "#fc7f5f", "#fb7d5d", "#fb7c5c", "#fb7b5b",
                "#fb7a5a", "#fb7858", "#fb7757", "#fb7656", "#fb7555", "#fb7353", "#fb7252", "#fb7151",
                "#fb7050", "#fb6e4e", "#fb6d4d", "#fb6c4c", "#fb6b4b", "#fb694a", "#fa6849", "#fa6648",
                "#fa6547", "#f96346", "#f96245", "#f96044", "#f85f43", "#f85f43", "#f85d42", "#f75c41",
                "#f75b40", "#f7593f", "#f6583e", "#f6563d", "#f6553c", "#f5533b", "#f5523a", "#f4503a",
                "#f44f39", "#f44d38", "#f34c37", "#f34a36", "#f34935", "#f24734", "#f24633", "#f14432",
                "#f14331", "#f14130", "#f0402f", "#f03f2e", "#f03d2d", "#ef3c2c", "#ef3c2c", "#ee3a2c",
                "#ed392b", "#ec382b", "#eb372a", "#ea362a", "#e93529", "#e83429", "#e63328", "#e53228",
                "#e43027", "#e32f27", "#e22e27", "#e12d26", "#e02c26", "#de2b25", "#dd2a25", "#dc2924",
                "#db2824", "#da2723", "#d92523", "#d82422", "#d72322", "#d52221", "#d42121", "#d32020",
                "#d32020", "#d21f20", "#d11e1f", "#d01d1f", "#cf1c1f", "#ce1a1e", "#cc191e", "#cb181d",
                "#ca181d", "#c9181d", "#c8171c", "#c7171c", "#c5171c", "#c4161c", "#c3161b", "#c2161b",
                "#c1161b", "#bf151b", "#be151a", "#bd151a", "#bc141a", "#bb141a", "#b91419", "#b81419",
                "#b71319", "#b71319", "#b61319", "#b51318", "#b31218", "#b21218", "#b11218", "#b01217",
                "#af1117", "#ad1117", "#ac1117", "#ab1016", "#aa1016", "#a91016", "#a81016", "#a60f15",
                "#a50f15", "#a30f15", "#a10e15", "#9f0e14", "#9d0d14", "#9c0d14", "#9a0c14", "#980c13",
                "#960b13", "#940b13", "#920a13", "#920a13", "#900a12", "#8e0912", "#8c0912", "#8a0812",
                "#880811", "#860811", "#840711", "#820711", "#800610", "#7e0610", "#7c0510", "#7a0510"
        ));

        public static ColorMapContainer RdYlBu = ColorMapContainer.FromStringList(new List<string>(
                "#a50026", "#a70226", "#a90426", "#ab0626", "#ad0826", "#af0926", "#b10b26", "#b30d26",
                "#b50f26", "#b71126", "#b91326", "#bb1526", "#bd1726", "#bd1726", "#be1827", "#c01a27",
                "#c21c27", "#c41e27", "#c62027", "#c82227", "#ca2427", "#cc2627", "#ce2827", "#d02927",
                "#d22b27", "#d42d27", "#d62f27", "#d83128", "#d93429", "#da362a", "#db382b", "#dc3b2c",
                "#dd3d2d", "#de402e", "#e0422f", "#e14430", "#e24731", "#e34933", "#e44c34", "#e44c34",
                "#e54e35", "#e65036", "#e75337", "#e95538", "#ea5739", "#eb5a3a", "#ec5c3b", "#ed5f3c",
                "#ee613e", "#ef633f", "#f16640", "#f26841", "#f36b42", "#f46d43", "#f47044", "#f57245",
                "#f57547", "#f57748", "#f67a49", "#f67c4a", "#f67f4b", "#f7814c", "#f7844e", "#f8864f",
                "#f8864f", "#f88950", "#f88c51", "#f98e52", "#f99153", "#f99355", "#fa9656", "#fa9857",
                "#fa9b58", "#fb9d59", "#fba05b", "#fba35c", "#fca55d", "#fca85e", "#fcaa5f", "#fdad60",
                "#fdaf62", "#fdb164", "#fdb366", "#fdb567", "#fdb769", "#fdb96b", "#fdbb6d", "#fdbd6f",
                "#fdbf71", "#fdc173", "#fdc173", "#fdc374", "#fdc576", "#fdc778", "#fec87a", "#feca7c",
                "#fecc7e", "#fece7f", "#fed081", "#fed283", "#fed485", "#fed687", "#fed889", "#feda8a",
                "#fedc8c", "#fede8e", "#fee090", "#fee192", "#fee294", "#fee496", "#fee597", "#fee699",
                "#fee79b", "#fee99d", "#feea9f", "#feea9f", "#feeba1", "#feeca2", "#feeda4", "#feefa6",
                "#fff0a8", "#fff1aa", "#fff2ac", "#fff3ad", "#fff5af", "#fff6b1", "#fff7b3", "#fff8b5",
                "#fffab7", "#fffbb9", "#fffcba", "#fffdbc", "#fffebe", "#feffc0", "#fdfec2", "#fcfec5",
                "#fbfdc7", "#fafdc9", "#f8fccb", "#f7fcce", "#f6fbd0", "#f6fbd0", "#f5fbd2", "#f3fbd4",
                "#f2fad6", "#f1fad9", "#f0f9db", "#eff9dd", "#edf8df", "#ecf8e2", "#ebf7e4", "#eaf7e6",
                "#e9f6e8", "#e7f6eb", "#e6f5ed", "#e5f5ef", "#e4f4f1", "#e2f4f4", "#e1f3f6", "#e0f3f8",
                "#def2f7", "#dcf1f7", "#daf0f6", "#d8eff6", "#d6eef5", "#d4edf4", "#d4edf4", "#d1ecf4",
                "#cfebf3", "#cdeaf3", "#cbe9f2", "#c9e8f2", "#c7e7f1", "#c5e6f0", "#c3e5f0", "#c1e4ef",
                "#bfe3ef", "#bde2ee", "#bbe1ed", "#b9e0ed", "#b6dfec", "#b4deec", "#b2ddeb", "#b0dcea",
                "#aedbea", "#acdae9", "#aad8e9", "#a8d6e8", "#a6d5e7", "#a3d3e6", "#a1d1e5", "#9fd0e4",
                "#9fd0e4", "#9dcee3", "#9bcce2", "#99cae1", "#97c9e0", "#94c7df", "#92c5de", "#90c3dd",
                "#8ec2dc", "#8cc0db", "#8abeda", "#87bdd9", "#85bbd9", "#83b9d8", "#81b7d7", "#7fb6d6",
                "#7db4d5", "#7ab2d4", "#78b0d3", "#76afd2", "#74add1", "#72abd0", "#70a9cf", "#6ea6ce",
                "#6da4cc", "#6da4cc", "#6ba2cb", "#69a0ca", "#679ec9", "#659bc8", "#6399c7", "#6297c6",
                "#6095c4", "#5e93c3", "#5c90c2", "#5a8ec1", "#588cc0", "#578abf", "#5588be", "#5385bd",
                "#5183bb", "#4f81ba", "#4d7fb9", "#4b7db8", "#4a7ab7", "#4878b6", "#4676b5", "#4574b3",
                "#4471b2", "#436fb1", "#426cb0", "#426cb0", "#416aaf", "#4167ad", "#4065ac", "#3f62ab",
                "#3e60aa", "#3e5ea8", "#3d5ba7", "#3c59a6", "#3b56a5", "#3a54a4", "#3a51a2", "#394fa1"
        ));

    }

}
