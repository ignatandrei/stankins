export interface AliveResult {
  process: string;
  arguments: string;
  to: string;
  isSuccess: boolean;
  result: string;
  duration: number;
  detailedResult: string;
  exception: string;
  hasError: string;
}
export interface CustomData {
  name: string;
  tags: string[];
}

export interface ResultWithData  {
  aliveResult: AliveResult;
  customData: CustomData;
}

