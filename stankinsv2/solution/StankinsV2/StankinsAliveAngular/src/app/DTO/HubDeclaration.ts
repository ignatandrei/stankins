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
  startedDate: Date;
}
export interface CustomData {
  name: string;
  tags: string[];
  icon: string;
}

export interface ResultWithData  {
  aliveResult: AliveResult;
  customData: CustomData;
  cronExecution: CRONExecution;
}
export interface CRONExecution {
  cron: string;
  nextRunTime: Date;

}
