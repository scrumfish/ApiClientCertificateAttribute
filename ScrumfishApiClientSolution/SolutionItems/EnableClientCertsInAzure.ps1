# place your subscription and site into the script.  
# Do not commit your subscription info.
# Update clientCertSetting.json to reflect your datacenter.
# Install the armclient: https://github.com/projectkudu/ARMClient

armclient login
armclient PUT subscriptions/**your subscription id**/resourcegroups/Default-Web-WestUS/providers/Microsoft.Web/sites/**your site**/?api-version=2014-04-01 "@clientCertSetting.json" -verbose

