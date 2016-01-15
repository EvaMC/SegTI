﻿/*	CAST-256 Block Cypher Implementation
	Copyright (C) 1999, Daniel Roethlisberger    
    Ported to C#: 2012, Daniel Schick    
 
	This program is free software; you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation; either version 2 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program; if not, write to the Free Software
	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

	The author of this program may be contacted at admin@roe.ch.
*/

using System;

namespace Cast256
{

    public class Cast256
    {

        internal const int CAST_BLOCK_SIZE = 128;
        internal const int CAST_USERKEY_SIZE = 128;
        internal const int CAST_ROUNDS = 12;

        // 128 bit BETA
        internal struct BETA
        {
            public uint A;
            public uint B;
            public uint C;
            public uint D;
        };

        // 256 bit KAPPA
        internal struct KAPPA
        {
            public uint A;
            public uint B;
            public uint C;
            public uint D;
            public uint E;
            public uint F;
            public uint G;
            public uint H;
        };


        // S-Boxes
        uint[] S1 = {
	0x30FB40D4, 0x9FA0FF0B, 0x6BECCD2F, 0x3F258C7A,
	0x1E213F2F, 0x9C004DD3, 0x6003E540, 0xCF9FC949,
	0xBFD4AF27, 0x88BBBDB5, 0xE2034090, 0x98D09675,
	0x6E63A0E0, 0x15C361D2, 0xC2E7661D, 0x22D4FF8E,
	0x28683B6F, 0xC07FD059, 0xFF2379C8, 0x775F50E2,
	0x43C340D3, 0xDF2F8656, 0x887CA41A, 0xA2D2BD2D,
	0xA1C9E0D6, 0x346C4819, 0x61B76D87, 0x22540F2F,
	0x2ABE32E1, 0xAA54166B, 0x22568E3A, 0xA2D341D0,
	0x66DB40C8, 0xA784392F, 0x004DFF2F, 0x2DB9D2DE,
	0x97943FAC, 0x4A97C1D8, 0x527644B7, 0xB5F437A7,
	0xB82CBAEF, 0xD751D159, 0x6FF7F0ED, 0x5A097A1F,
	0x827B68D0, 0x90ECF52E, 0x22B0C054, 0xBC8E5935,
	0x4B6D2F7F, 0x50BB64A2, 0xD2664910, 0xBEE5812D,
	0xB7332290, 0xE93B159F, 0xB48EE411, 0x4BFF345D,
	0xFD45C240, 0xAD31973F, 0xC4F6D02E, 0x55FC8165,
	0xD5B1CAAD, 0xA1AC2DAE, 0xA2D4B76D, 0xC19B0C50,
	0x882240F2, 0x0C6E4F38, 0xA4E4BFD7, 0x4F5BA272,
	0x564C1D2F, 0xC59C5319, 0xB949E354, 0xB04669FE,
	0xB1B6AB8A, 0xC71358DD, 0x6385C545, 0x110F935D,
	0x57538AD5, 0x6A390493, 0xE63D37E0, 0x2A54F6B3,
	0x3A787D5F, 0x6276A0B5, 0x19A6FCDF, 0x7A42206A,
	0x29F9D4D5, 0xF61B1891, 0xBB72275E, 0xAA508167,
	0x38901091, 0xC6B505EB, 0x84C7CB8C, 0x2AD75A0F,
	0x874A1427, 0xA2D1936B, 0x2AD286AF, 0xAA56D291,
	0xD7894360, 0x425C750D, 0x93B39E26, 0x187184C9,
	0x6C00B32D, 0x73E2BB14, 0xA0BEBC3C, 0x54623779,
	0x64459EAB, 0x3F328B82, 0x7718CF82, 0x59A2CEA6,
	0x04EE002E, 0x89FE78E6, 0x3FAB0950, 0x325FF6C2,
	0x81383F05, 0x6963C5C8, 0x76CB5AD6, 0xD49974C9,
	0xCA180DCF, 0x380782D5, 0xC7FA5CF6, 0x8AC31511,
	0x35E79E13, 0x47DA91D0, 0xF40F9086, 0xA7E2419E,
	0x31366241, 0x051EF495, 0xAA573B04, 0x4A805D8D,
	0x548300D0, 0x00322A3C, 0xBF64CDDF, 0xBA57A68E,
	0x75C6372B, 0x50AFD341, 0xA7C13275, 0x915A0BF5,
	0x6B54BFAB, 0x2B0B1426, 0xAB4CC9D7, 0x449CCD82,
	0xF7FBF265, 0xAB85C5F3, 0x1B55DB94, 0xAAD4E324,
	0xCFA4BD3F, 0x2DEAA3E2, 0x9E204D02, 0xC8BD25AC,
	0xEADF55B3, 0xD5BD9E98, 0xE31231B2, 0x2AD5AD6C,
	0x954329DE, 0xADBE4528, 0xD8710F69, 0xAA51C90F,
	0xAA786BF6, 0x22513F1E, 0xAA51A79B, 0x2AD344CC,
	0x7B5A41F0, 0xD37CFBAD, 0x1B069505, 0x41ECE491,
	0xB4C332E6, 0x032268D4, 0xC9600ACC, 0xCE387E6D,
	0xBF6BB16C, 0x6A70FB78, 0x0D03D9C9, 0xD4DF39DE,
	0xE01063DA, 0x4736F464, 0x5AD328D8, 0xB347CC96,
	0x75BB0FC3, 0x98511BFB, 0x4FFBCC35, 0xB58BCF6A,
	0xE11F0ABC, 0xBFC5FE4A, 0xA70AEC10, 0xAC39570A,
	0x3F04442F, 0x6188B153, 0xE0397A2E, 0x5727CB79,
	0x9CEB418F, 0x1CACD68D, 0x2AD37C96, 0x0175CB9D,
	0xC69DFF09, 0xC75B65F0, 0xD9DB40D8, 0xEC0E7779,
	0x4744EAD4, 0xB11C3274, 0xDD24CB9E, 0x7E1C54BD,
	0xF01144F9, 0xD2240EB1, 0x9675B3FD, 0xA3AC3755,
	0xD47C27AF, 0x51C85F4D, 0x56907596, 0xA5BB15E6,
	0x580304F0, 0xCA042CF1, 0x011A37EA, 0x8DBFAADB,
	0x35BA3E4A, 0x3526FFA0, 0xC37B4D09, 0xBC306ED9,
	0x98A52666, 0x5648F725, 0xFF5E569D, 0x0CED63D0,
	0x7C63B2CF, 0x700B45E1, 0xD5EA50F1, 0x85A92872,
	0xAF1FBDA7, 0xD4234870, 0xA7870BF3, 0x2D3B4D79,
	0x42E04198, 0x0CD0EDE7, 0x26470DB8, 0xF881814C,
	0x474D6AD7, 0x7C0C5E5C, 0xD1231959, 0x381B7298,
	0xF5D2F4DB, 0xAB838653, 0x6E2F1E23, 0x83719C9E,
	0xBD91E046, 0x9A56456E, 0xDC39200C, 0x20C8C571,
	0x962BDA1C, 0xE1E696FF, 0xB141AB08, 0x7CCA89B9,
	0x1A69E783, 0x02CC4843, 0xA2F7C579, 0x429EF47D,
	0x427B169C, 0x5AC9F049, 0xDD8F0F00, 0x5C8165BF
	};

        uint[] S2 = {
	0x1F201094, 0xEF0BA75B, 0x69E3CF7E, 0x393F4380,
	0xFE61CF7A, 0xEEC5207A, 0x55889C94, 0x72FC0651,
	0xADA7EF79, 0x4E1D7235, 0xD55A63CE, 0xDE0436BA,
	0x99C430EF, 0x5F0C0794, 0x18DCDB7D, 0xA1D6EFF3,
	0xA0B52F7B, 0x59E83605, 0xEE15B094, 0xE9FFD909,
	0xDC440086, 0xEF944459, 0xBA83CCB3, 0xE0C3CDFB,
	0xD1DA4181, 0x3B092AB1, 0xF997F1C1, 0xA5E6CF7B,
	0x01420DDB, 0xE4E7EF5B, 0x25A1FF41, 0xE180F806,
	0x1FC41080, 0x179BEE7A, 0xD37AC6A9, 0xFE5830A4,
	0x98DE8B7F, 0x77E83F4E, 0x79929269, 0x24FA9F7B,
	0xE113C85B, 0xACC40083, 0xD7503525, 0xF7EA615F,
	0x62143154, 0x0D554B63, 0x5D681121, 0xC866C359,
	0x3D63CF73, 0xCEE234C0, 0xD4D87E87, 0x5C672B21,
	0x071F6181, 0x39F7627F, 0x361E3084, 0xE4EB573B,
	0x602F64A4, 0xD63ACD9C, 0x1BBC4635, 0x9E81032D,
	0x2701F50C, 0x99847AB4, 0xA0E3DF79, 0xBA6CF38C,
	0x10843094, 0x2537A95E, 0xF46F6FFE, 0xA1FF3B1F,
	0x208CFB6A, 0x8F458C74, 0xD9E0A227, 0x4EC73A34,
	0xFC884F69, 0x3E4DE8DF, 0xEF0E0088, 0x3559648D,
	0x8A45388C, 0x1D804366, 0x721D9BFD, 0xA58684BB,
	0xE8256333, 0x844E8212, 0x128D8098, 0xFED33FB4,
	0xCE280AE1, 0x27E19BA5, 0xD5A6C252, 0xE49754BD,
	0xC5D655DD, 0xEB667064, 0x77840B4D, 0xA1B6A801,
	0x84DB26A9, 0xE0B56714, 0x21F043B7, 0xE5D05860,
	0x54F03084, 0x066FF472, 0xA31AA153, 0xDADC4755,
	0xB5625DBF, 0x68561BE6, 0x83CA6B94, 0x2D6ED23B,
	0xECCF01DB, 0xA6D3D0BA, 0xB6803D5C, 0xAF77A709,
	0x33B4A34C, 0x397BC8D6, 0x5EE22B95, 0x5F0E5304,
	0x81ED6F61, 0x20E74364, 0xB45E1378, 0xDE18639B,
	0x881CA122, 0xB96726D1, 0x8049A7E8, 0x22B7DA7B,
	0x5E552D25, 0x5272D237, 0x79D2951C, 0xC60D894C,
	0x488CB402, 0x1BA4FE5B, 0xA4B09F6B, 0x1CA815CF,
	0xA20C3005, 0x8871DF63, 0xB9DE2FCB, 0x0CC6C9E9,
	0x0BEEFF53, 0xE3214517, 0xB4542835, 0x9F63293C,
	0xEE41E729, 0x6E1D2D7C, 0x50045286, 0x1E6685F3,
	0xF33401C6, 0x30A22C95, 0x31A70850, 0x60930F13,
	0x73F98417, 0xA1269859, 0xEC645C44, 0x52C877A9,
	0xCDFF33A6, 0xA02B1741, 0x7CBAD9A2, 0x2180036F,
	0x50D99C08, 0xCB3F4861, 0xC26BD765, 0x64A3F6AB,
	0x80342676, 0x25A75E7B, 0xE4E6D1FC, 0x20C710E6,
	0xCDF0B680, 0x17844D3B, 0x31EEF84D, 0x7E0824E4,
	0x2CCB49EB, 0x846A3BAE, 0x8FF77888, 0xEE5D60F6,
	0x7AF75673, 0x2FDD5CDB, 0xA11631C1, 0x30F66F43,
	0xB3FAEC54, 0x157FD7FA, 0xEF8579CC, 0xD152DE58,
	0xDB2FFD5E, 0x8F32CE19, 0x306AF97A, 0x02F03EF8,
	0x99319AD5, 0xC242FA0F, 0xA7E3EBB0, 0xC68E4906,
	0xB8DA230C, 0x80823028, 0xDCDEF3C8, 0xD35FB171,
	0x088A1BC8, 0xBEC0C560, 0x61A3C9E8, 0xBCA8F54D,
	0xC72FEFFA, 0x22822E99, 0x82C570B4, 0xD8D94E89,
	0x8B1C34BC, 0x301E16E6, 0x273BE979, 0xB0FFEAA6,
	0x61D9B8C6, 0x00B24869, 0xB7FFCE3F, 0x08DC283B,
	0x43DAF65A, 0xF7E19798, 0x7619B72F, 0x8F1C9BA4,
	0xDC8637A0, 0x16A7D3B1, 0x9FC393B7, 0xA7136EEB,
	0xC6BCC63E, 0x1A513742, 0xEF6828BC, 0x520365D6,
	0x2D6A77AB, 0x3527ED4B, 0x821FD216, 0x095C6E2E,
	0xDB92F2FB, 0x5EEA29CB, 0x145892F5, 0x91584F7F,
	0x5483697B, 0x2667A8CC, 0x85196048, 0x8C4BACEA,
	0x833860D4, 0x0D23E0F9, 0x6C387E8A, 0x0AE6D249,
	0xB284600C, 0xD835731D, 0xDCB1C647, 0xAC4C56EA,
	0x3EBD81B3, 0x230EABB0, 0x6438BC87, 0xF0B5B1FA,
	0x8F5EA2B3, 0xFC184642, 0x0A036B7A, 0x4FB089BD,
	0x649DA589, 0xA345415E, 0x5C038323, 0x3E5D3BB9,
	0x43D79572, 0x7E6DD07C, 0x06DFDF1E, 0x6C6CC4EF,
	0x7160A539, 0x73BFBE70, 0x83877605, 0x4523ECF1
	};

        uint[] S3 = {
	0x8DEFC240, 0x25FA5D9F, 0xEB903DBF, 0xE810C907,
	0x47607FFF, 0x369FE44B, 0x8C1FC644, 0xAECECA90,
	0xBEB1F9BF, 0xEEFBCAEA, 0xE8CF1950, 0x51DF07AE,
	0x920E8806, 0xF0AD0548, 0xE13C8D83, 0x927010D5,
	0x11107D9F, 0x07647DB9, 0xB2E3E4D4, 0x3D4F285E,
	0xB9AFA820, 0xFADE82E0, 0xA067268B, 0x8272792E,
	0x553FB2C0, 0x489AE22B, 0xD4EF9794, 0x125E3FBC,
	0x21FFFCEE, 0x825B1BFD, 0x9255C5ED, 0x1257A240,
	0x4E1A8302, 0xBAE07FFF, 0x528246E7, 0x8E57140E,
	0x3373F7BF, 0x8C9F8188, 0xA6FC4EE8, 0xC982B5A5,
	0xA8C01DB7, 0x579FC264, 0x67094F31, 0xF2BD3F5F,
	0x40FFF7C1, 0x1FB78DFC, 0x8E6BD2C1, 0x437BE59B,
	0x99B03DBF, 0xB5DBC64B, 0x638DC0E6, 0x55819D99,
	0xA197C81C, 0x4A012D6E, 0xC5884A28, 0xCCC36F71,
	0xB843C213, 0x6C0743F1, 0x8309893C, 0x0FEDDD5F,
	0x2F7FE850, 0xD7C07F7E, 0x02507FBF, 0x5AFB9A04,
	0xA747D2D0, 0x1651192E, 0xAF70BF3E, 0x58C31380,
	0x5F98302E, 0x727CC3C4, 0x0A0FB402, 0x0F7FEF82,
	0x8C96FDAD, 0x5D2C2AAE, 0x8EE99A49, 0x50DA88B8,
	0x8427F4A0, 0x1EAC5790, 0x796FB449, 0x8252DC15,
	0xEFBD7D9B, 0xA672597D, 0xADA840D8, 0x45F54504,
	0xFA5D7403, 0xE83EC305, 0x4F91751A, 0x925669C2,
	0x23EFE941, 0xA903F12E, 0x60270DF2, 0x0276E4B6,
	0x94FD6574, 0x927985B2, 0x8276DBCB, 0x02778176,
	0xF8AF918D, 0x4E48F79E, 0x8F616DDF, 0xE29D840E,
	0x842F7D83, 0x340CE5C8, 0x96BBB682, 0x93B4B148,
	0xEF303CAB, 0x984FAF28, 0x779FAF9B, 0x92DC560D,
	0x224D1E20, 0x8437AA88, 0x7D29DC96, 0x2756D3DC,
	0x8B907CEE, 0xB51FD240, 0xE7C07CE3, 0xE566B4A1,
	0xC3E9615E, 0x3CF8209D, 0x6094D1E3, 0xCD9CA341,
	0x5C76460E, 0x00EA983B, 0xD4D67881, 0xFD47572C,
	0xF76CEDD9, 0xBDA8229C, 0x127DADAA, 0x438A074E,
	0x1F97C090, 0x081BDB8A, 0x93A07EBE, 0xB938CA15,
	0x97B03CFF, 0x3DC2C0F8, 0x8D1AB2EC, 0x64380E51,
	0x68CC7BFB, 0xD90F2788, 0x12490181, 0x5DE5FFD4,
	0xDD7EF86A, 0x76A2E214, 0xB9A40368, 0x925D958F,
	0x4B39FFFA, 0xBA39AEE9, 0xA4FFD30B, 0xFAF7933B,
	0x6D498623, 0x193CBCFA, 0x27627545, 0x825CF47A,
	0x61BD8BA0, 0xD11E42D1, 0xCEAD04F4, 0x127EA392,
	0x10428DB7, 0x8272A972, 0x9270C4A8, 0x127DE50B,
	0x285BA1C8, 0x3C62F44F, 0x35C0EAA5, 0xE805D231,
	0x428929FB, 0xB4FCDF82, 0x4FB66A53, 0x0E7DC15B,
	0x1F081FAB, 0x108618AE, 0xFCFD086D, 0xF9FF2889,
	0x694BCC11, 0x236A5CAE, 0x12DECA4D, 0x2C3F8CC5,
	0xD2D02DFE, 0xF8EF5896, 0xE4CF52DA, 0x95155B67,
	0x494A488C, 0xB9B6A80C, 0x5C8F82BC, 0x89D36B45,
	0x3A609437, 0xEC00C9A9, 0x44715253, 0x0A874B49,
	0xD773BC40, 0x7C34671C, 0x02717EF6, 0x4FEB5536,
	0xA2D02FFF, 0xD2BF60C4, 0xD43F03C0, 0x50B4EF6D,
	0x07478CD1, 0x006E1888, 0xA2E53F55, 0xB9E6D4BC,
	0xA2048016, 0x97573833, 0xD7207D67, 0xDE0F8F3D,
	0x72F87B33, 0xABCC4F33, 0x7688C55D, 0x7B00A6B0,
	0x947B0001, 0x570075D2, 0xF9BB88F8, 0x8942019E,
	0x4264A5FF, 0x856302E0, 0x72DBD92B, 0xEE971B69,
	0x6EA22FDE, 0x5F08AE2B, 0xAF7A616D, 0xE5C98767,
	0xCF1FEBD2, 0x61EFC8C2, 0xF1AC2571, 0xCC8239C2,
	0x67214CB8, 0xB1E583D1, 0xB7DC3E62, 0x7F10BDCE,
	0xF90A5C38, 0x0FF0443D, 0x606E6DC6, 0x60543A49,
	0x5727C148, 0x2BE98A1D, 0x8AB41738, 0x20E1BE24,
	0xAF96DA0F, 0x68458425, 0x99833BE5, 0x600D457D,
	0x282F9350, 0x8334B362, 0xD91D1120, 0x2B6D8DA0,
	0x642B1E31, 0x9C305A00, 0x52BCE688, 0x1B03588A,
	0xF7BAEFD5, 0x4142ED9C, 0xA4315C11, 0x83323EC5,
	0xDFEF4636, 0xA133C501, 0xE9D3531C, 0xEE353783
	};

        uint[] S4 = {
	0x9DB30420, 0x1FB6E9DE, 0xA7BE7BEF, 0xD273A298,
	0x4A4F7BDB, 0x64AD8C57, 0x85510443, 0xFA020ED1,
	0x7E287AFF, 0xE60FB663, 0x095F35A1, 0x79EBF120,
	0xFD059D43, 0x6497B7B1, 0xF3641F63, 0x241E4ADF,
	0x28147F5F, 0x4FA2B8CD, 0xC9430040, 0x0CC32220,
	0xFDD30B30, 0xC0A5374F, 0x1D2D00D9, 0x24147B15,
	0xEE4D111A, 0x0FCA5167, 0x71FF904C, 0x2D195FFE,
	0x1A05645F, 0x0C13FEFE, 0x081B08CA, 0x05170121,
	0x80530100, 0xE83E5EFE, 0xAC9AF4F8, 0x7FE72701,
	0xD2B8EE5F, 0x06DF4261, 0xBB9E9B8A, 0x7293EA25,
	0xCE84FFDF, 0xF5718801, 0x3DD64B04, 0xA26F263B,
	0x7ED48400, 0x547EEBE6, 0x446D4CA0, 0x6CF3D6F5,
	0x2649ABDF, 0xAEA0C7F5, 0x36338CC1, 0x503F7E93,
	0xD3772061, 0x11B638E1, 0x72500E03, 0xF80EB2BB,
	0xABE0502E, 0xEC8D77DE, 0x57971E81, 0xE14F6746,
	0xC9335400, 0x6920318F, 0x081DBB99, 0xFFC304A5,
	0x4D351805, 0x7F3D5CE3, 0xA6C866C6, 0x5D5BCCA9,
	0xDAEC6FEA, 0x9F926F91, 0x9F46222F, 0x3991467D,
	0xA5BF6D8E, 0x1143C44F, 0x43958302, 0xD0214EEB,
	0x022083B8, 0x3FB6180C, 0x18F8931E, 0x281658E6,
	0x26486E3E, 0x8BD78A70, 0x7477E4C1, 0xB506E07C,
	0xF32D0A25, 0x79098B02, 0xE4EABB81, 0x28123B23,
	0x69DEAD38, 0x1574CA16, 0xDF871B62, 0x211C40B7,
	0xA51A9EF9, 0x0014377B, 0x041E8AC8, 0x09114003,
	0xBD59E4D2, 0xE3D156D5, 0x4FE876D5, 0x2F91A340,
	0x557BE8DE, 0x00EAE4A7, 0x0CE5C2EC, 0x4DB4BBA6,
	0xE756BDFF, 0xDD3369AC, 0xEC17B035, 0x06572327,
	0x99AFC8B0, 0x56C8C391, 0x6B65811C, 0x5E146119,
	0x6E85CB75, 0xBE07C002, 0xC2325577, 0x893FF4EC,
	0x5BBFC92D, 0xD0EC3B25, 0xB7801AB7, 0x8D6D3B24,
	0x20C763EF, 0xC366A5FC, 0x9C382880, 0x0ACE3205,
	0xAAC9548A, 0xECA1D7C7, 0x041AFA32, 0x1D16625A,
	0x6701902C, 0x9B757A54, 0x31D477F7, 0x9126B031,
	0x36CC6FDB, 0xC70B8B46, 0xD9E66A48, 0x56E55A79,
	0x026A4CEB, 0x52437EFF, 0x2F8F76B4, 0x0DF980A5,
	0x8674CDE3, 0xEDDA04EB, 0x17A9BE04, 0x2C18F4DF,
	0xB7747F9D, 0xAB2AF7B4, 0xEFC34D20, 0x2E096B7C,
	0x1741A254, 0xE5B6A035, 0x213D42F6, 0x2C1C7C26,
	0x61C2F50F, 0x6552DAF9, 0xD2C231F8, 0x25130F69,
	0xD8167FA2, 0x0418F2C8, 0x001A96A6, 0x0D1526AB,
	0x63315C21, 0x5E0A72EC, 0x49BAFEFD, 0x187908D9,
	0x8D0DBD86, 0x311170A7, 0x3E9B640C, 0xCC3E10D7,
	0xD5CAD3B6, 0x0CAEC388, 0xF73001E1, 0x6C728AFF,
	0x71EAE2A1, 0x1F9AF36E, 0xCFCBD12F, 0xC1DE8417,
	0xAC07BE6B, 0xCB44A1D8, 0x8B9B0F56, 0x013988C3,
	0xB1C52FCA, 0xB4BE31CD, 0xD8782806, 0x12A3A4E2,
	0x6F7DE532, 0x58FD7EB6, 0xD01EE900, 0x24ADFFC2,
	0xF4990FC5, 0x9711AAC5, 0x001D7B95, 0x82E5E7D2,
	0x109873F6, 0x00613096, 0xC32D9521, 0xADA121FF,
	0x29908415, 0x7FBB977F, 0xAF9EB3DB, 0x29C9ED2A,
	0x5CE2A465, 0xA730F32C, 0xD0AA3FE8, 0x8A5CC091,
	0xD49E2CE7, 0x0CE454A9, 0xD60ACD86, 0x015F1919,
	0x77079103, 0xDEA03AF6, 0x78A8565E, 0xDEE356DF,
	0x21F05CBE, 0x8B75E387, 0xB3C50651, 0xB8A5C3EF,
	0xD8EEB6D2, 0xE523BE77, 0xC2154529, 0x2F69EFDF,
	0xAFE67AFB, 0xF470C4B2, 0xF3E0EB5B, 0xD6CC9876,
	0x39E4460C, 0x1FDA8538, 0x1987832F, 0xCA007367,
	0xA99144F8, 0x296B299E, 0x492FC295, 0x9266BEAB,
	0xB5676E69, 0x9BD3DDDA, 0xDF7E052F, 0xDB25701C,
	0x1B5E51EE, 0xF65324E6, 0x6AFCE36C, 0x0316CC04,
	0x8644213E, 0xB7DC59D0, 0x7965291F, 0xCCD6FD43,
	0x41823979, 0x932BCDF6, 0xB657C34D, 0x4EDFD282,
	0x7AE5290C, 0x3CB9536B, 0x851E20FE, 0x9833557E,
	0x13ECF0B0, 0xD3FFB372, 0x3F85C5C1, 0x0AEF7ED2
	};


        #region helper funcs
        /// <summary>
        /// rotate left operation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private uint ROTL(uint x, uint n)
        {
            return (((x) << ((int)n)) | ((x) >> (32 - ((int)n))));
        }

        // Extract the nth BYTE from a uint
        private byte ext_a(uint x)
        {
            return ((byte)((x >> 24) & ((uint)0xFF)));
        }

        private byte ext_b(uint x)
        {
            return ((byte)((x >> 16) & 0xFF));
        }

        private byte ext_c(uint x)
        {
            return ((byte)((x >> 8) & 0xFF));
        }

        private byte ext_d(uint x)
        {
            return ((byte)(x & 0xFF));
        }

        private uint FIVE_LSB(uint x)
        {
            return ((x) & (uint)0x0000001F);
        }

        #endregion

        // ========================================================================
        //  The global rotation and mask round subkey generation tables
        // ------------------------------------------------------------------------
        private KAPPA[] g_Tr = new KAPPA[24];
        private KAPPA[] g_Tm = new KAPPA[24];

        // ========================================================================
        //  The F-Functions
        // ------------------------------------------------------------------------
        uint f1(uint D, uint Kri, uint Kmi)
        {
            uint I = this.ROTL((Kmi + D), Kri);
            return ((S1[this.ext_a(I)] ^ S2[this.ext_b(I)]) - S3[this.ext_c(I)]) + S4[this.ext_d(I)];
        }
        uint f2(uint D, uint Kri, uint Kmi)
        {
            uint I = ROTL((Kmi ^ D), Kri);
            return ((S1[this.ext_a(I)] - S2[this.ext_b(I)]) + S3[this.ext_c(I)]) ^ S4[this.ext_d(I)];
        }
        uint f3(uint D, uint Kri, uint Kmi)
        {
            uint I = ROTL((Kmi - D), Kri);
            return ((S1[this.ext_a(I)] + S2[this.ext_b(I)]) ^ S3[this.ext_c(I)]) - S4[this.ext_d(I)];
        }

        // ========================================================================
        //  The Quad-Round and Reverse Quad Round
        // ------------------------------------------------------------------------
        BETA Q(int round, BETA data, BETA[] Kr, BETA[] Km)
        {

            data.C = data.C ^ f1(data.D, Kr[round].A, Km[round].A);
            data.B = data.B ^ f2(data.C, Kr[round].B, Km[round].B);
            data.A = data.A ^ f3(data.B, Kr[round].C, Km[round].C);
            data.D = data.D ^ f1(data.A, Kr[round].D, Km[round].D);

            return data;
        }
        BETA QBAR(int round, BETA data, BETA[] Kr, BETA[] Km)
        {
            data.D = data.D ^ f1(data.A, Kr[round].D, Km[round].D);
            data.A = data.A ^ f3(data.B, Kr[round].C, Km[round].C);
            data.B = data.B ^ f2(data.C, Kr[round].B, Km[round].B);
            data.C = data.C ^ f1(data.D, Kr[round].A, Km[round].A);

            return data;
        }

        // ========================================================================
        //  The Forward Octave
        // ------------------------------------------------------------------------
        KAPPA W(int round, KAPPA data, KAPPA[] Tr, KAPPA[] Tm)
        {
            data.G = data.G ^ f1(data.H, Tr[round].A, Tm[round].A);
            data.F = data.F ^ f2(data.G, Tr[round].B, Tm[round].B);
            data.E = data.E ^ f3(data.F, Tr[round].C, Tm[round].C);
            data.D = data.D ^ f1(data.E, Tr[round].D, Tm[round].D);
            data.C = data.C ^ f2(data.D, Tr[round].E, Tm[round].E);
            data.B = data.B ^ f3(data.C, Tr[round].F, Tm[round].F);
            data.A = data.A ^ f1(data.B, Tr[round].G, Tm[round].G);
            data.H = data.H ^ f2(data.A, Tr[round].H, Tm[round].H);

            return data;
        }

        // ========================================================================
        //  Encrypt a 128bit block
        // ------------------------------------------------------------------------
        internal void CAST256Encrypt(BETA[] Kr, BETA[] Km, ref BETA pData)
        {
            for (int i = 0; i < 6; i++)
                pData = Q(i, pData, Kr, Km);
            for (int i = 6; i < 12; i++)
                pData = QBAR(i, pData, Kr, Km);
        }

        // ========================================================================
        //  Decrypt a 128bit block
        // ------------------------------------------------------------------------
        internal void CAST256Decrypt(BETA[] Kr, BETA[] Km, ref BETA pData)
        {
            for (int i = 11; i >= 6; i--)
                pData = Q(i, pData, Kr, Km);
            for (int i = 5; i >= 0; i--)
                pData = QBAR(i, pData, Kr, Km);
        }

        // ========================================================================
        //  Initialize the mask and rotation round subkey sets from a given user key
        // ------------------------------------------------------------------------
        internal void CAST256KeyInit(BETA[] Kr, BETA[] Km, KAPPA userKey)
        {
            for (int i = 0; i < 12; i++)
            {
                userKey = W(2 * i, userKey, g_Tr, g_Tm);
                userKey = W(2 * i + 1, userKey, g_Tr, g_Tm);

                Kr[i].A = FIVE_LSB(userKey.A);
                Kr[i].B = FIVE_LSB(userKey.C);
                Kr[i].C = FIVE_LSB(userKey.E);
                Kr[i].D = FIVE_LSB(userKey.G);

                Km[i].A = userKey.H;
                Km[i].B = userKey.F;
                Km[i].C = userKey.D;
                Km[i].D = userKey.B;
            }
        }

        // ========================================================================
        //  Initialize the tables used for key initialization / expansion
        // ------------------------------------------------------------------------
        public void CAST256TableInit()
        {
            uint Cm = 0x5A827999;
            uint Mm = 0x6ED9EBA1;
            uint Cr = 19;
            uint Mr = 17;
            for (int i = 0; i < 24; i++)
            {
                g_Tm[i].A = Cm;
                Cm += Mm;
                g_Tm[i].B = Cm;
                Cm += Mm;
                g_Tm[i].C = Cm;
                Cm += Mm;
                g_Tm[i].D = Cm;
                Cm += Mm;
                g_Tm[i].E = Cm;
                Cm += Mm;
                g_Tm[i].F = Cm;
                Cm += Mm;
                g_Tm[i].G = Cm;
                Cm += Mm;
                g_Tm[i].H = Cm;
                Cm += Mm;
                g_Tr[i].A = Cr;
                Cr = FIVE_LSB(Cr + Mr);
                g_Tr[i].B = Cr;
                Cr = FIVE_LSB(Cr + Mr);
                g_Tr[i].C = Cr;
                Cr = FIVE_LSB(Cr + Mr);
                g_Tr[i].D = Cr;
                Cr = FIVE_LSB(Cr + Mr);
                g_Tr[i].E = Cr;
                Cr = FIVE_LSB(Cr + Mr);
                g_Tr[i].F = Cr;
                Cr = FIVE_LSB(Cr + Mr);
                g_Tr[i].G = Cr;
                Cr = FIVE_LSB(Cr + Mr);
                g_Tr[i].H = Cr;
                Cr = FIVE_LSB(Cr + Mr);
            }
        }


        internal struct auxiliar
        {
            public string A;
            public string B;
            public string C;
            public string D;
        };

        private auxiliar res1 = new auxiliar();
        private auxiliar res2 = new auxiliar();
        private auxiliar aux1 = new auxiliar();
        private auxiliar aux2 = new auxiliar();

        internal const string ZERO1 = "0";
        internal const string ZERO2 = "00";
        internal const string ZERO3 = "000";
        internal const string ZERO4 = "0000";
        internal const string ZERO5 = "00000";
        internal const string ZERO6 = "000000";
        internal const string ZERO7 = "0000000";
        internal const string ZERO8 = "00000000";
        internal const string ZERO9 = "000000000";


        private auxiliar t = new auxiliar();

        internal auxiliar completaString(ref BETA pData)
        {
            int tamA = 0;
            int tamB = 0;
            int tamC = 0;
            int tamD = 0;

            t.A = pData.A.ToString();
            t.B = pData.B.ToString();
            t.C = pData.C.ToString();
            t.D = pData.D.ToString();

            tamA = t.A.Length;
            tamB = t.B.Length;
            tamC = t.C.Length;
            tamD = t.D.Length;

            int diffA = 10 - tamA;
            int diffB = 10 - tamB;
            int diffC = 10 - tamC;
            int diffD = 10 - tamD;


            switch (diffA)
            {
                case 1:
                    t.A = ZERO1 + t.A;
                    break;
                case 2:
                    t.A = ZERO2 + t.A;
                    break;
                case 3:
                    t.A = ZERO3 + t.A;
                    break;
                case 4:
                    t.A = ZERO4 + t.A;
                    break;
                case 5:
                    t.A = ZERO5 + t.A;
                    break;
                case 6:
                    t.A = ZERO6 + t.A;
                    break;
                case 7:
                    t.A = ZERO7 + t.A;
                    break;
                case 8:
                    t.A = ZERO8 + t.A;
                    break;
                case 9:
                    t.A = ZERO9 + t.A;
                    break;

            }
            switch (diffB)
            {
                case 1:
                    t.B = ZERO1 + t.B;
                    break;
                case 2:
                    t.B = ZERO2 + t.B;
                    break;
                case 3:
                    t.B = ZERO3 + t.B;
                    break;
                case 4:
                    t.B = ZERO4 + t.B;
                    break;
                case 5:
                    t.B = ZERO5 + t.B;
                    break;
                case 6:
                    t.B = ZERO6 + t.B;
                    break;
                case 7:
                    t.B = ZERO7 + t.B;
                    break;
                case 8:
                    t.B = ZERO8 + t.B;
                    break;
                case 9:
                    t.B = ZERO9 + t.B;
                    break;

            }
            switch (diffC)
            {
                case 1:
                    t.C = ZERO1 + t.C;
                    break;
                case 2:
                    t.C = ZERO2 + t.C;
                    break;
                case 3:
                    t.C = ZERO3 + t.C;
                    break;
                case 4:
                    t.C = ZERO4 + t.C;
                    break;
                case 5:
                    t.C = ZERO5 + t.C;
                    break;
                case 6:
                    t.C = ZERO6 + t.C;
                    break;
                case 7:
                    t.C = ZERO7 + t.C;
                    break;
                case 8:
                    t.C = ZERO8 + t.C;
                    break;
                case 9:
                    t.C = ZERO9 + t.C;
                    break;

            }
            switch (diffD)
            {
                case 1:
                    t.D = ZERO1 + t.D;
                    break;
                case 2:
                    t.D = ZERO2 + t.D;
                    break;
                case 3:
                    t.D = ZERO3 + t.D;
                    break;
                case 4:
                    t.D = ZERO4 + t.D;
                    break;
                case 5:
                    t.D = ZERO5 + t.D;
                    break;
                case 6:
                    t.D = ZERO6 + t.D;
                    break;
                case 7:
                    t.D = ZERO7 + t.D;
                    break;
                case 8:
                    t.D = ZERO8 + t.D;
                    break;
                case 9:
                    t.D = ZERO9 + t.D;
                    break;

            }
            return t;
        }

        internal int calcularDist(ref auxiliar res1,ref auxiliar res2)
        {
            int dist = 0;

            aux1.A = res1.A.ToString();
            aux1.B = res1.B.ToString();
            aux1.C = res1.C.ToString();
            aux1.D = res1.D.ToString();

            aux2.A = res2.A.ToString();
            aux2.B = res2.B.ToString();
            aux2.C = res2.C.ToString();
            aux2.D = res2.D.ToString();

            for (int i = 0; i < 10; i++)
            {
                if (aux1.A[i] != aux2.A[i])
                    dist += 1;
            }
            for (int j = 0; j < 10; j++)
            {
                if (aux1.A[j] != aux2.A[j])
                    dist += 1;
            }

            for (int k = 0; k < 10; k++)
            {
                if (aux2.C[k] != aux1.C[k])
                    dist += 1;
            }

            for (int l = 0; l < 10; l++)
            {
                //Console.WriteLine(aux2.B.Length+"     "+l+ "   "+aux2.B[l]+" | "+aux1.B[l]);

                if (aux2.B[l] != aux1.B[l])
                    dist += 1;
            }

            for (int m = 0; m < 10; m++)
            {
                if (aux1.D[m] != aux2.D[m])
                    dist += 1;
            }

            return dist;
        }

        internal int distanciaHamming(ref BETA pData1 , ref BETA pData2)
        {
            int dist = 0;

            //Usamos la funcion completarString
            /*Console.WriteLine("TEXTO CIFRADO");
            Console.WriteLine("1.1__"+pData1.A+"  "+pData1.B+"  "+pData1.C+"  "+pData1.D);
            Console.WriteLine("2.1__"+pData2.A+"  "+pData2.B+"  "+pData2.C+"  "+pData2.D);*/

            res1 = completaString(ref pData1);
            res2 = completaString(ref pData2);

            /*Console.WriteLine("TRAS COMPLETAR STRINGS");
            Console.WriteLine("1.2__"+res1.A+"  "+res1.B+"  "+res1.C+"  "+res1.D);
            Console.WriteLine("2.2__"+res2.A+"  "+res2.B+"  "+res2.C+"  "+res2.D);*/

            return dist = calcularDist(ref res1,ref res2);
        }
    }
}

