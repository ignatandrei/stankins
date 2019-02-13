copy npm-shrinkwrap.json src/npm-shrinkwrap.json /Y
call ng build versions-netcore-angular
cd dist/versions-netcore-angular
npm publish
