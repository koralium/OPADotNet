docker run -it --rm -v ${PWD}:/go/src/regosdk -w /go/src/regosdk/go/src -e CGO_ENABLED=1 docker.elastic.co/beats-dev/golang-crossbuild:1.16.4-main --build-cmd "go build -o ../../netcore/src/OPADotNet.Embedded/runtimes/win-x86/native/regosdk.dll -buildmode=c-shared" -p "windows/386"
rm netcore/src/OPADotNet.Embedded/runtimes/win-x86/native/regosdk.h
docker run -it --rm -v ${PWD}:/go/src/regosdk -w /go/src/regosdk/go/src -e CGO_ENABLED=1 docker.elastic.co/beats-dev/golang-crossbuild:1.16.4-main --build-cmd "go build -o ../../netcore/src/OPADotNet.Embedded/runtimes/win-x64/native/regosdk.dll -buildmode=c-shared" -p "windows/amd64"
rm netcore/src/OPADotNet.Embedded/runtimes/win-x64/native/regosdk.h