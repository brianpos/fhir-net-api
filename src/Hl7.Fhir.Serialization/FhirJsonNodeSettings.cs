﻿/* 
 * Copyright (c) 2018, Firely (info@fire.ly) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://github.com/ewoutkramer/fhir-net-api/blob/master/LICENSE
 */


namespace Hl7.Fhir.Serialization
{
    public class FhirJsonNodeSettings
    {
        public bool PermissiveParsing;
        public bool AllowJsonComments;

#if !NETSTANDARD1_1
        public bool ValidateFhirXhtml;
#endif

        public FhirJsonNodeSettings Clone() =>
            new FhirJsonNodeSettings
            {
                PermissiveParsing = PermissiveParsing,
                AllowJsonComments = AllowJsonComments,
#if !NETSTANDARD1_1
                ValidateFhirXhtml = ValidateFhirXhtml
#endif
            };


    }
}