cd go/src
go build -o ../../netcore/src/OPADotNet.Embedded/runtimes/osx-x64/native/regosdk.dylib -buildmode=c-shared
cd ../..
rm netcore/src/OPADotNet.Embedded/runtimes/osx-x64/native/regosdk.h