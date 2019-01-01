export interface FileVersionInfo {
  isPreRelease: boolean;
  productPrivatePart: number;
  productName: string;
  productMinorPart: number;
  productMajorPart: number;
  productBuildPart: number;
  privateBuild: string;
  originalFilename: string;
  legalTrademarks: string;
  legalCopyright: string;
  language: string;
  isSpecialBuild: boolean;
  isPrivateBuild: boolean;
  productVersion: string;
  specialBuild: string;
  isDebug: boolean;
  internalName: string;
  fileVersion: string;
  filePrivatePart: number;
  fileName: string;
  fileMinorPart: number;
  fileMajorPart: number;
  fileDescription: string;
  fileBuildPart: number;
  companyName: string;
  comments: string;
  isPatched: boolean;
}

export class FVSAngular {
  name: string;
  version: string;
  dev: boolean;
}
