#!/bin/bash
rm -f appsettings.json
sed -e "s#{awsRegion}#$AWS_REGION#" appsettings.json.template > appsettings.json
sed -i -e "s#{awsAccessKey}#$AWS_ACCESS_KEY_ID#" appsettings.json
sed -i -e "s#{awsSecretKey}#$AWS_SECRET_ACCESS_KEY#" appsettings.json
sed -i -e "s#{appAddress}#$ADDRESS_APP#" appsettings.json

dotnet vTrade.IdentityServer.dll