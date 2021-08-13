from sphinx_diagrams import SphinxDiagram
from diagrams import Cluster
from diagrams.custom import Custom
from urllib.request import urlretrieve
from diagrams.k8s.compute import Pod
from diagrams.k8s.network import SVC
from diagrams.azure.database import BlobStorage
from diagrams.azure.devops import Repos, Pipelines
from diagrams.azure.integration import APIManagement
from diagrams.azure.database import SQLDatabases

with SphinxDiagram(title="OPA Server Sync"):
    opaserver_url = "https://raw.githubusercontent.com/open-policy-agent/opa/main/logo/logo-144x144.png"
    opaserver_icon = "diagrams.png"
    urlretrieve(opaserver_url, opaserver_icon)

    opa_server = Custom("OPA Server", opaserver_icon)
    blobStore = BlobStorage("BlobStore: Data & Policies")
    gateway = APIManagement("API Gateway")
    cicd = Pipelines("CI/CD Pipeline")
    dataservice = Pod("Data service")

    with Cluster("OPADotNet Service A"):
      svcA_group = [Pod("Pod-1"),
                  Pod("Pod-2")]
      svcA_service = SVC("Service")

      svcA_group << svcA_service

    with Cluster("OPADotNet Service B"):
      svcB_group = [Pod("Pod-1")]
      svcB_service = SVC("Service")

      svcB_group << svcB_service

    opa_server << svcA_group
    opa_server << svcB_group

    Repos("Policy Repo") >> cicd
    cicd >> blobStore
    blobStore << opa_server

    SQLDatabases("Database") << dataservice
    dataservice >> blobStore

    svcA_service << gateway
    svcB_service << gateway