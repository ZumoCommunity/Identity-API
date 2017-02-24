makecert -n "CN=ZumoCommunity.IdentityApi" -a sha256 -sv IdentityApi.pvk -r IdentityApi.cer 
pvk2pfx -pvk IdentityApi.pvk -spc IdentityApi.cer -pfx IdentityApi.pfx -pi 1d3nt1tyAp1