#!/bin/bash
rm -f /usr/share/nginx/html/assets/config.json
sed -e "s#{apiAddress}#$ADDRESS_API#" /usr/share/nginx/html/assets/config.json.template > /usr/share/nginx/html/assets/config.json
sed -i -e "s#{appAddress}#$ADDRESS_APP#" /usr/share/nginx/html/assets/config.json 
sed -i -e "s#{ssoAddress}#$ADDRESS_SSO#" /usr/share/nginx/html/assets/config.json 

nginx -g "daemon off;"