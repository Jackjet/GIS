﻿using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using wContour;

namespace wContourToSliverlight
{
    /// <summary>
    /// 等值线类
    /// author:du
    /// date;20130805
    /// </summary>
    public partial class MainPageForGIS : UserControl
    {

        double[,] _gridData = null;
        double[,] _discreteData = null;
        double[] _X = null;
        double[] _Y = null;
        double[] _CValues = null;
        Color[] _colors = null;

        bool bCB_DiscreteData = false;
        bool bCB_GridData = false;
        bool bCB_BorderLines = false;
        bool bCB_ContourLine = false;
        bool bCB_ContourPolygon = false;
        bool bCB_Clipped = true;//是否Clip
       
        List<List<PointD>> _mapLines = new List<List<PointD>>();
        List<wContour.Border> _borders = new List<wContour.Border>();
        List<PolyLine> _contourLines = new List<PolyLine>();
        List<PolyLine> _clipContourLines = new List<PolyLine>();
        List<wContour.Polygon> _contourPolygons = new List<wContour.Polygon>();
        List<wContour.Polygon> _clipContourPolygons = new List<wContour.Polygon>();
        List<wContour.Legend.lPolygon> _legendPolygons = new List<Legend.lPolygon>();
        List<PolyLine> _streamLines = new List<PolyLine>();
        
        double _undefData = -9999.0;
        List<List<PointD>> _clipLines = new List<List<PointD>>();
        Color _startColor = default(Color);
        Color _endColor = default(Color);
        private int _highlightIdx = 0;

        private double _minX = 0;
        private double _minY = 0;
        private double _maxX = 0;
        private double _maxY = 0;
        private double _scaleX = 1.0;
        private double _scaleY = 1.0;

        public string _dFormat = "0";

        public string gstrRegionLineCoords = "118.02953, 29.171082;118.047256, 29.215599;118.067631, 29.216156;118.079937, 29.231288;118.073917, 29.284826;118.139336, 29.284714;118.176613, 29.297363;118.1688, 29.312096;118.208485, 29.351939;118.208782, 29.376272;118.194305, 29.388206;118.222477, 29.422712;118.25, 29.430623;118.312542, 29.421191;118.307072, 29.492722;118.326527, 29.496572;118.344978, 29.475667;118.383034, 29.510096;118.432335, 29.504072;118.448982, 29.513359;118.479038, 29.510902;118.496078, 29.520607;118.500591, 29.57562;118.532181, 29.588931;118.569317, 29.635475;118.624348, 29.652273;118.640953, 29.641899;118.650997, 29.646652;118.69289, 29.699125;118.724079, 29.715932;118.74614, 29.744486;118.738632, 29.80792;118.763451, 29.820246;118.750866, 29.830846;118.759365, 29.846718;118.787617, 29.846636;118.841347, 29.891281;118.837837, 29.936554;118.872177, 29.946047;118.893982, 29.94147;118.889851, 30.010564;118.902153, 30.027832;118.868862, 30.101433;118.896961, 30.144371;118.846249, 30.158735;118.908833, 30.187192;118.927494, 30.209908;118.905209, 30.216543;118.883877, 30.246625;118.8792, 30.312154;118.949776, 30.358739;118.985737, 30.34952;118.988377, 30.332815;119.059875, 30.303813;119.09026, 30.323987;119.18821, 30.291621;119.215435, 30.299967;119.225498, 30.288762;119.247135, 30.34078;119.277092, 30.341161;119.323837, 30.370959;119.354733, 30.353569;119.403053, 30.373294;119.369293, 30.384328;119.365414, 30.407277;119.348347, 30.415281;119.327213, 30.532557;119.279037, 30.509939;119.242588, 30.529994;119.238045, 30.549037;119.2645, 30.577202;119.242008, 30.614133;119.314453, 30.623245;119.343159, 30.664164;119.389668, 30.685567;119.412689, 30.642939;119.443722, 30.649826;119.446999, 30.670647;119.482765, 30.704287;119.479584, 30.77121;119.525993, 30.777283;119.549332, 30.819374;119.576653, 30.831416;119.55727, 30.875837;119.558022, 30.904495;119.577126, 30.926722;119.585892, 30.976357;119.630062, 31.013326;119.624752, 31.0815;119.649467, 31.107628;119.633667, 31.126958;119.663368, 31.16541;119.69836, 31.153361;119.71508, 31.169042;119.779774, 31.178686;119.793423, 31.168068;119.789261, 31.159693;119.809837, 31.161395;119.81995, 31.17323;119.883137, 31.161489;119.920483, 31.170512;119.993622, 31.03315;120.057896, 31.002075;120.127716, 30.94632;120.259537, 30.92703;120.368748, 30.947853;120.357075, 30.90889;120.367107, 30.883296;120.413727, 30.894567;120.417801, 30.92409;120.432865, 30.921661;120.434502, 30.887993;120.448742, 30.873868;120.441269, 30.858274;120.459034, 30.840988;120.457401, 30.816088;120.499813, 30.759677;120.561356, 30.834531;120.590763, 30.854192;120.643837, 30.857658;120.649521, 30.850639;120.68111, 30.879707;120.704296, 30.872953;120.710392, 30.929296;120.686001, 30.955045;120.697456, 30.969892;120.748035, 30.963687;120.792351, 31.003487;120.863949, 30.990308;120.889446, 31.002449;120.895145, 31.017362;120.936543, 31.009254;120.957573, 31.028501;120.992054, 31.00618;121.003544, 30.909409;120.990982, 30.896573;121.019886, 30.877663;121.012779, 30.852056;120.991497, 30.836784;120.994274, 30.821401;121.017078, 30.833333;121.037094, 30.816233;121.06216, 30.846232;121.111095, 30.855517;121.137936, 30.825661;121.129173, 30.778341;121.176193, 30.772977;121.216972, 30.786138;121.232632, 30.756994;121.268738, 30.729945;121.269893, 30.698445;121.237598, 30.666667;121.156753, 30.624625;121.145979, 30.602697;121.120467, 30.592318;121.083401, 30.597591;121.074523, 30.581131;121.004329, 30.560046;120.971682, 30.535662;120.939329, 30.476925;120.959368, 30.427257;120.92541, 30.403429;120.914371, 30.371738;120.867339, 30.366022;120.849631, 30.35298;120.84199, 30.313184;120.732454, 30.291393;120.712604, 30.307791;120.705562, 30.370838;120.672767, 30.396506;120.653553, 30.368181;120.662951, 30.359696;120.65967, 30.333735;120.703817, 30.25;120.743872, 30.215961;120.813162, 30.225684;120.844175, 30.166667;120.866016, 30.155551;120.910239, 30.164028;120.923618, 30.171619;120.919303, 30.182901;120.974032, 30.195922;121.084933, 30.272502;121.162483, 30.306886;121.178073, 30.332198;121.215148, 30.31768;121.25, 30.320202;121.266033, 30.33628;121.287947, 30.34041;121.317882, 30.334965;121.36217, 30.319905;121.351276, 30.301378;121.444907, 30.254829;121.5, 30.209256;121.5274, 30.151975;121.541022, 30.15819;121.527782, 30.150955;121.542541, 30.133186;121.568244, 30.145692;121.554104, 30.166667;121.596092, 30.105401;121.582969, 30.085082;121.600063, 30.05464;121.622997, 30.070247;121.663405, 30.027457;121.748628, 29.977624;121.790464, 29.970608;121.852289, 29.933203;121.915065, 29.921363;121.944681, 29.88814;122.011921, 29.888251;122.023231, 29.879004;122.028602, 29.887485;122.079188, 29.888791;122.116541, 29.905727;122.144091, 29.88707;122.135775, 29.873802;121.979026, 29.807515;121.889761, 29.736799;121.858728, 29.691393;121.838173, 29.696653;121.838234, 29.697342;121.828624, 29.699097;121.838173, 29.696653;121.836806, 29.68135;121.782806, 29.651651;121.740124, 29.592752;121.724486, 29.584809;121.709076, 29.593906;121.715441, 29.565419;121.607977, 29.537051;121.572977, 29.54811;121.521704, 29.503661;121.517478, 29.554742;121.484332, 29.537044;121.475158, 29.512715;121.466417, 29.521287;121.448874, 29.517105;121.46395, 29.48962;121.430267, 29.469273;121.449527, 29.438177;121.441623, 29.411617;121.471107, 29.41793;121.484316, 29.468889;121.513813, 29.488741;121.537418, 29.463433;121.518993, 29.420898;121.493826, 29.409661;121.487099, 29.393231;121.494631, 29.387986;121.547741, 29.421942;121.547259, 29.446797;121.570564, 29.471784;121.636871, 29.491398;121.650987, 29.479957;121.644412, 29.489032;121.653681, 29.497331;121.730421, 29.538375;121.761367, 29.527229;121.777797, 29.535228;121.767444, 29.4811;121.782103, 29.468264;121.81209, 29.476756;121.844455, 29.538746;121.809355, 29.549119;121.747867, 29.546202;121.802615, 29.602068;121.896662, 29.620254;121.925039, 29.64036;121.93867, 29.628759;121.955569, 29.631484;121.960814, 29.617014;121.97916, 29.612235;121.979835, 29.596921;121.995712, 29.592984;121.977797, 29.57492;121.985293, 29.568717;121.958563, 29.561309;121.960204, 29.538639;121.942335, 29.519961;121.960295, 29.506801;121.960432, 29.475547;121.989596, 29.444885;121.98122, 29.449019;121.970362, 29.434674;121.971743, 29.418609;121.941836, 29.426217;121.922001, 29.418207;121.943457, 29.394299;121.922949, 29.359204;121.924685, 29.338739;121.950579, 29.336578;121.948822, 29.320638;121.930862, 29.316402;121.932509, 29.289045;121.968262, 29.274334;121.993639, 29.277832;121.997726, 29.262995;121.970011, 29.259844;121.962329, 29.25;121.976816, 29.220145;121.97013, 29.21439;121.958529, 29.226526;121.934271, 29.19066;121.854186, 29.160206;121.806688, 29.194683;121.83469, 29.235345;121.821658, 29.265011;121.809301, 29.252697;121.798542, 29.267105;121.815506, 29.312227;121.822956, 29.306766;121.823596, 29.318288;121.84143, 29.319377;121.84145, 29.327357;121.81531, 29.335191;121.795849, 29.31724;121.805636, 29.349959;121.794299, 29.361866;121.809824, 29.372352;121.78459, 29.362029;121.777793, 29.368969;121.771567, 29.353815;121.788783, 29.347322;121.785705, 29.276803;121.777029, 29.280807;121.786823, 29.265024;121.763064, 29.187578;121.735705, 29.186395;121.698557, 29.16849;121.662799, 29.171007;121.631521, 29.186487;121.605736, 29.261239;121.578132, 29.267513;121.57288, 29.260545;121.547522, 29.275339;121.554119, 29.259041;121.582426, 29.243461;121.581037, 29.218234;121.547682, 29.232886;121.532752, 29.201018;121.526042, 29.206443;121.537548, 29.185399;121.507763, 29.154954;121.498802, 29.156727;121.496628, 29.192129;121.487598, 29.197567;121.488307, 29.163919;121.472747, 29.154629;121.447771, 29.165664;121.441951, 29.158989;121.469833, 29.129253;121.452854, 29.109553;121.459926, 29.101302;121.508811, 29.099125;121.574583, 29.115642;121.653145, 29.102075;121.636181, 29.084082;121.641332, 29.051975;121.63066, 29.03924;121.592819, 29.039629;121.586669, 29.053876;121.547904, 29.049796;121.531341, 29.057582;121.548941, 29.030885;121.535434, 29.000182;121.561793, 29.009594;121.55707, 29.043902;121.578911, 29.045237;121.594366, 29.031664;121.631111, 29.033891;121.660927, 29.050734;121.669249, 29.045414;121.664286, 29.023915;121.681029, 29.024182;121.682537, 29.032146;121.683535, 29.020035;121.699707, 29.023108;121.704863, 29.015221;121.681686, 29.010575;121.689681, 29.005714;121.675109, 28.970028;121.701849, 28.96337;121.69549, 28.948309;121.710506, 28.958121;121.707411, 28.941463;121.734683, 28.942552;121.704436, 28.914959;121.664855, 28.898388;121.580282, 28.924811;121.542075, 28.953125;121.52161, 28.937606;121.551094, 28.923668;121.554794, 28.905304;121.588919, 28.907046;121.598116, 28.896181;121.632834, 28.8899;121.648249, 28.878364;121.641113, 28.859266;121.671387, 28.857159;121.668479, 28.834138;121.686732, 28.829768;121.667724, 28.811586;121.686782, 28.785419;121.649696, 28.776693;121.644462, 28.718466;121.663108, 28.719865;121.656771, 28.713529;121.632511, 28.718411;121.621463, 28.706072;121.583713, 28.704891;121.547858, 28.689312;121.487951, 28.691827;121.488895, 28.674353;121.518737, 28.652678;121.532257, 28.583333;121.560404, 28.534277;121.569872, 28.52477;121.602027, 28.523587;121.621829, 28.541042;121.630809, 28.517993;121.612875, 28.488913;121.587435, 28.490226;121.616469, 28.469846;121.604191, 28.460421;121.592886, 28.467014;121.581458, 28.448305;121.608854, 28.392574;121.601439, 28.364644;121.617797, 28.362331;121.63222, 28.339453;121.65681, 28.351092;121.654549, 28.342619;121.665087, 28.345106;121.637118, 28.292196;121.643446, 28.286341;121.620807, 28.272337;121.629557, 28.263474;121.588019, 28.246293;121.596248, 28.267457;121.577075, 28.266254;121.583724, 28.281806;121.575747, 28.300649;121.519644, 28.326382;121.468841, 28.326662;121.467005, 28.302311;121.454707, 28.295723;121.43191, 28.305449;121.412886, 28.284049;121.438205, 28.263668;121.408199, 28.213924;121.39783, 28.213312;121.399855, 28.205382;121.382858, 28.220497;121.367346, 28.213003;121.392444, 28.17635;121.364889, 28.135892;121.338974, 28.148294;121.334555, 28.166667;121.315883, 28.165456;121.294718, 28.190957;121.26178, 28.148203;121.266884, 28.123581;121.28923, 28.101322;121.280634, 28.080807;121.294449, 28.071534;121.251013, 28.082847;121.234679, 28.071565;121.252036, 28.066217;121.250731, 28.05745;121.222708, 28.067609;121.218294, 28.050553;121.159021, 28.033318;121.151606, 28.051632;121.159306, 28.06803;121.140278, 28.096063;121.135386, 28.128835;121.162645, 28.13525;121.202376, 28.16727;121.233149, 28.219091;121.258442, 28.222326;121.277617, 28.189121;121.280536, 28.197684;121.258001, 28.243893;121.27133, 28.255056;121.25482, 28.247517;121.232767, 28.257865;121.242244, 28.282498;121.221801, 28.333333;121.25, 28.340868;121.248344, 28.348917;121.220232, 28.343299;121.207344, 28.383555;121.184742, 28.383116;121.177741, 28.375434;121.186853, 28.353362;121.158724, 28.34411;121.150935, 28.317567;121.134655, 28.309729;121.136513, 28.293533;121.11214, 28.276148;121.134243, 28.24526;121.123336, 28.214212;121.102661, 28.195922;121.107559, 28.172018;121.064499, 28.157064;121.015833, 28.112309;121.004538, 28.044939;120.966678, 27.978197;120.884796, 27.995069;120.823672, 27.98513;120.875, 27.927962;120.868685, 27.863772;120.842971, 27.840664;120.835707, 27.853969;120.805601, 27.830074;120.81464, 27.818774;120.790977, 27.790302;120.778952, 27.797793;120.741927, 27.734518;120.684852, 27.69508;120.663162, 27.655009;120.680705, 27.637498;120.660167, 27.609549;120.625, 27.58959;120.618398, 27.596849;120.59877, 27.590599;120.625128, 27.4923;120.65921, 27.497671;120.677669, 27.47255;120.686942, 27.473698;120.669036, 27.44349;120.651385, 27.441905;120.640608, 27.404486;120.654792, 27.384102;120.651801, 27.361634;120.624301, 27.361916;120.617088, 27.387884;120.590838, 27.379737;120.551719, 27.401549;120.556105, 27.388023;120.547667, 27.390805;120.545111, 27.376461;120.563438, 27.376743;120.536506, 27.355725;120.539894, 27.341404;120.515178, 27.335684;120.527561, 27.323757;120.545977, 27.326636;120.562324, 27.311984;120.549197, 27.301916;120.567244, 27.293628;120.547671, 27.287385;120.545642, 27.27628;120.526348, 27.27457;120.519603, 27.246834;120.538016, 27.242086;120.533118, 27.221345;120.517231, 27.215132;120.522493, 27.192083;120.478954, 27.163429;120.464644, 27.18434;120.470106, 27.207763;120.457602, 27.219631;120.424379, 27.192059;120.438468, 27.169878;120.416782, 27.190837;120.411877, 27.23749;120.403076, 27.24144;120.430096, 27.259171;120.35199, 27.344961;120.346195, 27.394864;120.321033, 27.398443;120.322922, 27.40696;120.27335, 27.389394;120.262066, 27.430959;120.252663, 27.434168;120.219196, 27.420987;120.204178, 27.428085;120.139729, 27.42141;120.132702, 27.39474;120.103199, 27.392988;120.051979, 27.344556;120.035545, 27.342941;120, 27.37792;119.971436, 27.367788;119.93861, 27.329951;119.946411, 27.314669;119.9104, 27.318881;119.879822, 27.310415;119.878674, 27.302251;119.87225, 27.314415;119.841873, 27.322032;119.842003, 27.299515;119.76606, 27.308161;119.771271, 27.326373;119.777706, 27.320234;119.782772, 27.327326;119.768028, 27.346644;119.75, 27.351143;119.740349, 27.385684;119.686676, 27.436844;119.709087, 27.462163;119.704655, 27.518778;119.687256, 27.538643;119.659328, 27.539594;119.677143, 27.571513;119.630898, 27.582662;119.625336, 27.614784;119.647537, 27.640516;119.63858, 27.666262;119.610432, 27.674566;119.592777, 27.66566;119.57349, 27.670217;119.561787, 27.656985;119.54578, 27.666323;119.523266, 27.652008;119.497555, 27.65478;119.501835, 27.610713;119.470226, 27.525238;119.435509, 27.507673;119.421539, 27.531542;119.373623, 27.536471;119.343704, 27.51241;119.342625, 27.484507;119.290581, 27.460088;119.2766, 27.43236;119.25124, 27.421275;119.199242, 27.419426;119.13353, 27.431712;119.125, 27.435041;119.126377, 27.478742;119.066437, 27.466934;119.015362, 27.496145;118.988319, 27.477219;118.95977, 27.477018;118.958385, 27.451726;118.902004, 27.462931;118.876785, 27.514018;118.857109, 27.518036;118.859077, 27.526471;118.895908, 27.528753;118.909374, 27.56616;118.903233, 27.59007;118.913471, 27.61645;118.871239, 27.694077;118.89225, 27.71712;118.872204, 27.734421;118.848934, 27.786102;118.814373, 27.91526;118.792423, 27.932676;118.756413, 27.941826;118.729927, 27.972555;118.732925, 28.027084;118.718864, 28.047356;118.71896, 28.064112;118.741894, 28.087685;118.798447, 28.115781;118.805836, 28.15554;118.795235, 28.170306;118.763954, 28.165789;118.763924, 28.173991;118.811474, 28.223025;118.804043, 28.238327;118.755379, 28.252319;118.726608, 28.287287;118.72868, 28.303286;118.71159, 28.312848;118.675213, 28.272456;118.639515, 28.272959;118.620525, 28.255898;118.601078, 28.254336;118.586197, 28.284325;118.549408, 28.286415;118.527534, 28.270566;118.495602, 28.27505;118.486965, 28.267667;118.498058, 28.251729;118.487255, 28.239746;118.444939, 28.251038;118.43404, 28.287272;118.461151, 28.300977;118.486366, 28.33757;118.436367, 28.406044;118.476955, 28.476049;118.420597, 28.495003;118.418842, 28.503209;118.446114, 28.510859;118.430771, 28.518247;118.426765, 28.547325;118.409676, 28.568408;118.432205, 28.62205;118.418915, 28.64844;118.430813, 28.659996;118.430252, 28.679309;118.421463, 28.676844;118.405392, 28.698835;118.369614, 28.805659;118.309692, 28.819753;118.295883, 28.832184;118.302513, 28.838984;118.27317, 28.913691;118.258038, 28.924159;118.201191, 28.903389;118.198483, 28.916667;118.228798, 28.944311;118.202324, 28.953529;118.173767, 28.98557;118.098759, 28.990781;118.085918, 29.030914;118.069893, 29.038147;118.075287, 29.069839;118.039406, 29.094536;118.053089, 29.116213;118.040802, 29.13206;118.042961, 29.159297;118.02953, 29.171082";


        #region 开始等值线
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////
        /// </summary>
        private void DoContour()
        {
            ClearObjects();
            int rows = 100;
            int cols = 100;
            //_startColor = Color.FromArgb(255, 255, 255, 224);
            //_endColor = Color.FromArgb(255, 255, 0, 0);

            CreateDiscreteData(50);
            InterpolateData(rows, cols);

            double[] values = new double[] {
                10,
                50,
                100,
                150,
                200,
                250,
                300,
                500
            };
            SetContourValues(values);
            TracingContourLines();
            SmoothLines();
            GetEcllipseClipping();
            ClipLines();
            TracingPolygons();
            ClipPolygons();

            bCB_DiscreteData = false;
            bCB_GridData = false;
            bCB_BorderLines = false;
            bCB_ContourLine = false;
            bCB_ContourPolygon = true;
            _dFormat = "0";

            // CreateLegend();
           
            PaintGraphics();//开始画等值线

        }
        #endregion

        #region 生成离散数据
        public void CreateDiscreteData(int dataNum)
        {
            int i = 0;            
            double[,] S = null;
           
            dataNum = gstationGraphicsLayer.Graphics.Count;//???
            //---- Generate discrete points
            Random random = new Random();
            S = new double[3, dataNum];
            //---- x,y,value
            for (i = 0; i < dataNum; i++)
            {
                GetPointInfo.PointInfo pi=   gstationGraphicsLayer.Graphics[i].Attributes["tag"]  as GetPointInfo.PointInfo;
                S[0, i] = gstationGraphicsLayer.Graphics[i].Geometry.Extent.XMin;
                S[1, i] = gstationGraphicsLayer.Graphics[i].Geometry.Extent.YMin;
                S[2, i] = getAQIByPoint(pi.CityCode, pi.PCode); 
                //S[2, i] = random.Next(10, 100);
            }   
            _discreteData = S;
        }


        public void InterpolateData(int rows, int cols)
        {                      
            double[,] dataArray = null;
         
            //---- Generate Grid Coordinate           
            double Xlb = 0;
            double Ylb = 0;
            double Xrt = 0;
            double Yrt = 0;

            double dMinX=0, dMinY=0, dMaxX=0, dMaxY=0;

            getMinMaxXY(gstationGraphicsLayer, out dMinX, out dMinY, out dMaxX, out  dMaxY);
            Xrt = dMaxX;
            Yrt = dMaxY;

            Xlb = dMinX;
            Ylb = dMinY;

            Interpolate.CreateGridXY_Num(Xlb, Ylb, Xrt, Yrt, cols, rows, ref _X, ref _Y);

            dataArray = new double[rows, cols];
            dataArray = Interpolate.Interpolation_IDW_Neighbor(_discreteData, _X, _Y, 8, _undefData);

            _gridData = dataArray;
        }

        /// <summary>
        /// 获取最小的点和最大的点
        /// author:du
        /// date:20130801
        /// </summary>
        /// <param name="sender"></param>
        private void getMinMaxXY(GraphicsLayer gl, out double dMinX, out double dMinY, out double dMaxX, out double dMaxY)
        {

            dMinX = gl.Graphics[0].Geometry.Extent.XMin;
            dMinY = gl.Graphics[0].Geometry.Extent.YMin;
            dMaxX = gl.Graphics[0].Geometry.Extent.XMax;
            dMaxY = gl.Graphics[0].Geometry.Extent.YMax;

            string[] arrSplit = gstrRegionLineCoords.Split(new Char[] { ';' });
            List<PointD> cLine = new List<PointD>();

            for (int i = 0; i < arrSplit.Length; i++)
            {
                string[] arrCoord = arrSplit[i].Split(new Char[] { ',' });

                ESRI.ArcGIS.Client.Geometry.Geometry pPoint = mercator.FromGeographic(new MapPoint(System.Convert.ToDouble(arrCoord[0].ToString()), System.Convert.ToDouble(arrCoord[1].ToString())));

                if (pPoint.Extent.XMin < dMinX)
                {
                    dMinX = pPoint.Extent.XMin;
                }
                if (pPoint.Extent.YMin < dMinY)
                {
                    dMinY = pPoint.Extent.YMin;
                }

                if (pPoint.Extent.XMax > dMaxX)
                {
                    dMaxX = pPoint.Extent.XMax;
                }
                if (pPoint.Extent.YMax > dMaxY)
                {
                    dMaxY = pPoint.Extent.YMax;
                }
            }
        }
        //private void getMinMaxXY(GraphicsLayer gl, out double dMinX, out double dMinY, out double dMaxX, out double dMaxY)
        //{

        //    //double dMinX, dMinY, dMaxX, dMaxY;
        //    dMinX = gl.Graphics[0].Geometry.Extent.XMin;
        //    dMinY = gl.Graphics[0].Geometry.Extent.YMin;
        //    dMaxX = gl.Graphics[0].Geometry.Extent.XMax;
        //    dMaxY = gl.Graphics[0].Geometry.Extent.YMax;

        //    for (int i = 1; i < gl.Graphics.Count; i++)
        //    {
        //        if (gl.Graphics[i].Geometry.Extent.XMin < dMinX)
        //        {
        //            dMinX = gl.Graphics[i].Geometry.Extent.XMin;
        //        }
        //        if (gl.Graphics[i].Geometry.Extent.YMin < dMinY)
        //        {
        //            dMinY = gl.Graphics[i].Geometry.Extent.YMin;
        //        }

        //        if (gl.Graphics[i].Geometry.Extent.XMax > dMaxX)
        //        {
        //            dMaxX = gl.Graphics[i].Geometry.Extent.XMax;
        //        }
        //        if (gl.Graphics[i].Geometry.Extent.YMax > dMaxY)
        //        {
        //            dMaxY = gl.Graphics[i].Geometry.Extent.YMax;
        //        }

        //    }

        //}

        #endregion
        public void SetContourValues(double[] values)
        {
            _CValues = values;
        }

        public void TracingContourLines()
        {
            //---- Contour values
            int nc = _CValues.Length;

            //---- Colors
           // _colors = CreateColors(_startColor, _endColor, nc + 1);
               
            int[,] S1 = new int[1, 1];
            _borders = Contour.TracingBorders(_gridData, _X, _Y, ref S1, _undefData);
            _contourLines = Contour.TracingContourLines(_gridData, _X, _Y, nc, _CValues, _undefData, _borders, S1);
        }

        private Color[] CreateColors(Color sColor, Color eColor, int cNum)
        {
            Color[] colors = new Color[cNum];
            int sR = 0;
            int sG = 0;
            int sB = 0;
            int eR = 0;
            int eG = 0;
            int eB = 0;
            int rStep = 0;
            int gStep = 0;
            int bStep = 0;
            int i = 0;

            sR = sColor.R;
            sG = sColor.G;
            sB = sColor.B;
            eR = eColor.R;
            eG = eColor.G;
            eB = eColor.B;
            rStep = Convert.ToInt32((eR - sR) / cNum);
            gStep = Convert.ToInt32((eG - sG) / cNum);
            bStep = Convert.ToInt32((eB - sB) / cNum);
            for (i = 0; i <= colors.Length - 1; i++)
            {
                //colors[i] = Color.FromArgb(sR + i * rStep, sG + i * gStep, sB + i * bStep);
                colors[i] = Color.FromArgb(255, (byte)(sR + i * rStep), (byte)(sG + i * gStep), (byte)(sB + i * bStep));
            }

            return colors;
        }

        #region 开始画等值面
        void AddContourPLineGraphics(Point[] points,Color pColor)
        {
           
               ESRI.ArcGIS.Client.Projection.WebMercator mercator =new ESRI.ArcGIS.Client.Projection.WebMercator();

            ESRI.ArcGIS.Client.Geometry.Polyline polyline = new ESRI.ArcGIS.Client.Geometry.Polyline();
            ESRI.ArcGIS.Client.Geometry.PointCollection pointCollection = new ESRI.ArcGIS.Client.Geometry.PointCollection();
            pointCollection.Clear();
            for (int i = 0; i < points.Length; i++)
            {
                pointCollection.Add(new MapPoint(points[i].X,points[i].Y));
 
            }

            polyline.Paths.Add(pointCollection);

            SimpleLineSymbol pSymbol = LayoutRoot.Resources["DefaultLineSymbol"] as SimpleLineSymbol;
            pSymbol.Color =  new SolidColorBrush(pColor);
            
            SimpleLineSymbol ls = new SimpleLineSymbol(Colors.Black,2);
           

                Graphic graphic = new Graphic()
                {
                    //Geometry = mercator.FromGeographic(polyline),
                    Geometry = mercator.ToGeographic(polyline),
                    Symbol=pSymbol
                    //Symbol = LayoutRoot.Resources["DefaultLineSymbol"] as Symbol
                };
                gcontourGraphicsLayer.Graphics.Add(graphic);
           

        }
        void AddContourPolygonGraphics(Point[] points,Color pColor)
        {
            gcontourGraphicsLayer.Opacity = 0.5;

            ESRI.ArcGIS.Client.Geometry.Polygon polygon1 = new ESRI.ArcGIS.Client.Geometry.Polygon();
            ESRI.ArcGIS.Client.Geometry.PointCollection pointCollection = new ESRI.ArcGIS.Client.Geometry.PointCollection();
            pointCollection.Clear();
            for (int i = 0; i < points.Length; i++)
            {
                pointCollection.Add(new MapPoint(points[i].X, points[i].Y));
            }
            polygon1.Rings.Add(pointCollection);
      
            FillSymbol pSymbol = new FillSymbol();
            pSymbol.BorderThickness = 0.01;
            pSymbol.Fill = new SolidColorBrush(pColor);

            Graphic graphic = new Graphic()
            {
                Geometry = polygon1,
                Symbol=pSymbol
            };
            gcontourGraphicsLayer.Graphics.Add(graphic);
        }

        private Color getPolygonColor(double dLowValue,double dHighValue)
        {
            Color aColor = Colors.Black;

            if (dHighValue >= 0 && dHighValue <= 50)
            {
                aColor = Color.FromArgb((byte)255, (byte)0, (byte)228, (byte)0);
            }

            if (dHighValue >=51 && dHighValue <= 100)
            {
                aColor = Color.FromArgb((byte)255, (byte)255, (byte)255, (byte)0);
            }

            if (dHighValue >= 101 && dHighValue <= 150)
            {
                aColor = Color.FromArgb((byte)255, (byte)255, (byte)126, (byte)0);
            }

            if (dHighValue >= 151 && dHighValue <= 200)
            {
                aColor = Color.FromArgb((byte)255, (byte)255, (byte)0, (byte)0);
            }

            if (dHighValue >= 201 && dHighValue <= 300)
            {
                aColor = Color.FromArgb((byte)255, (byte)153, (byte)0, (byte)76);
            }

            if (dHighValue >= 301)
            {
                aColor = Color.FromArgb((byte)255, (byte)126, (byte)0, (byte)35);
            }

            return aColor;
        }
       
        /// <summary>
        /// 画等值面
        /// author:du
        /// date;20130819
        /// </summary>
        private void PaintGraphics()
        {
            int i = 0;
            int j = 0;
            wContour.PolyLine aline = default(wContour.PolyLine);
            List<PointD> newPList = new List<PointD>();
            double aValue = 0;
            Color aColor = default(Color);
       
            wContour.PointD aPoint = default(wContour.PointD);
            Point[] Points = null;
            int sX = 0;
            int sY = 0;

            bCB_ContourPolygon = true;//
            //Draw contour polygons
            if (bCB_ContourPolygon==true)
            {
                List<wContour.Polygon> drawPolygons = _contourPolygons;
                if (bCB_Clipped)
                    drawPolygons = _clipContourPolygons;

                wContour.Polygon aPolygon = default(wContour.Polygon);
                
                for (i = 0; i <= drawPolygons.Count - 1; i++)
                {
                    aPolygon = drawPolygons[i];
                    aline = aPolygon.OutLine;

                    aValue = aPolygon.LowValue;
                    
                    aColor = getPolygonColor(aPolygon.LowValue, aPolygon.HighValue);
                    newPList = aline.PointList;
                    if (!Contour.IsClockwise(newPList))
                        newPList.Reverse();
                    Points = new Point[newPList.Count];
                    for (j = 0; j <= newPList.Count - 1; j++)
                    {
                        aPoint = newPList[j];
                        Points[j] = new Point(aPoint.X, aPoint.Y );
                    }

                    AddContourPolygonGraphics(Points, aColor);
                    if (aPolygon.HasHoles)
                    {
                        for (int h = 0; h < aPolygon.HoleLines.Count; h++)
                        {
                            newPList = aPolygon.HoleLines[h].PointList;
                            if (Contour.IsClockwise(newPList))
                                newPList.Reverse();
                            Points = new Point[newPList.Count];
                            for (j = 0; j <= newPList.Count - 1; j++)
                            {
                                aPoint = newPList[j];
                                Points[j] = new Point(sX, sY);
                            }

                            AddContourPolygonGraphics(Points, aColor);

                        }
                    }
                }
            }

            bCB_ContourLine = false;
            //Draw contour lines
            if (bCB_ContourLine == true)
            {
                List<PolyLine> drawLines = _contourLines;
                if (bCB_Clipped)
                    drawLines = _clipContourLines;
                
                for (i = 0; i <= drawLines.Count - 1; i++)
                {
                    aline = drawLines[i];
                    aValue = aline.Value;
                    aColor = _colors[Array.IndexOf(_CValues, aValue)];
                    newPList = aline.PointList;

                    Points = new Point[newPList.Count];
                    for (j = 0; j <= newPList.Count - 1; j++)
                    {
                        aPoint = (wContour.PointD)newPList[j];
                        Points[j] = new Point(aPoint.X,aPoint.Y );
                    }
                    //aPen = new Pen(Color.Black);
                    //aPen.Color = aColor;
                    //g.DrawLines(aPen, Points);
                   // System.Windows.Shapes.Polyline pline2 = new System.Windows.Shapes.Polyline();
                    //pline2.Fill = new SolidColorBrush(aColor);
                    //pline2.Stroke = new SolidColorBrush(Colors.Black);
                   // pline2.StrokeThickness = 3;
                  AddContourPLineGraphics(Points, aColor);
                }
            }
        }
        #endregion
        public void ClearObjects()
        {


            gWaterLineGraphicsLayer.Graphics.Clear();


        }

        private void ToScreen(double pX, double pY, ref int sX, ref int sY)
        {
            sX = (int)((pX - _minX) * _scaleX);
            sY = (int)((_maxY - pY) * _scaleY);
        }

        private void ToScreen(double pX, double pY, ref float sX, ref float sY)
        {
            sX = (float)((pX - _minX) * _scaleX);
            sY = (float)((_maxY - pY) * _scaleY);
        }

        public void SmoothLines()
        {
            _contourLines = Contour.SmoothLines(_contourLines);
        }
        public void GetEcllipseClipping()
        {
            _clipLines = new List<List<PointD>>();

            string[] arrSplit = gstrRegionLineCoords.Split(new Char[] { ';' });
            List<PointD> cLine = new List<PointD>();
          
            string[] arrCoord=null;
            for (int i = 0; i < arrSplit.Length; i++)
            {
                arrCoord = arrSplit[i].Split(new Char[] { ',' });

                ESRI.ArcGIS.Client.Geometry.Geometry pPoint = mercator.FromGeographic(new MapPoint(System.Convert.ToDouble(arrCoord[0].ToString()), System.Convert.ToDouble(arrCoord[1].ToString())));

                wContour.PointD aPoint = default(wContour.PointD);
                aPoint.X = pPoint.Extent.XMin;
                aPoint.Y = pPoint.Extent.YMin;

                cLine.Add(aPoint);
            }
            _clipLines.Add(cLine);
        }

        public void ClipLines()
        {
            _clipContourLines = new List<PolyLine>();
            foreach (List<PointD> cLine in _clipLines)
                _clipContourLines.AddRange(Contour.ClipPolylines(_contourLines, cLine));
        }

        public void ClipPolygons()
        {
            _clipContourPolygons = new List<wContour.Polygon>();
            foreach (List<PointD> cLine in _clipLines)
                _clipContourPolygons.AddRange(Contour.ClipPolygons(_contourPolygons, cLine));
        }
        public void TracingPolygons()
        {           
            _contourPolygons = Contour.TracingPolygons(_gridData, _contourLines, _borders, _CValues);            
        }
        public void CreateLegend()
        {
            wContour.Legend aLegend = new wContour.Legend();
            wContour.PointD aPoint = default(wContour.PointD);

            double width = _maxX - _minX;
            aPoint.X = _minX + width / 4;
            aPoint.Y = _minY + width / 100;
            wContour.Legend.legendPara lPara = new Legend.legendPara();
            lPara.startPoint = aPoint;
            lPara.isTriangle = true;
            lPara.isVertical = false;
            lPara.length = width / 2;
            lPara.width = width / 100;
            lPara.contourValues = _CValues;

            _legendPolygons = Legend.CreateLegend(lPara);
        }
    }
}
