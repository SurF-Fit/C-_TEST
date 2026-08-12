export interface ValueRecord {
    id: string;
    date: Date;
    executionTime: number;
    value: number;
    fileName: string;
    createdAt: Date;
}

export interface ResultRecord{
    id: string;
    fileName: string;
    deltaTimeSeconds: number;
    minDate: Date;
    averageExecutionTime: number;
    averageValue: number;
    medianValue: number;
    maxValue: number;
    minValue: number;
    createAt: Date;
}

export interface ResultFilters {
    fileName?: string;
    minDate?: string;
    maxDate?: string;
    minAvgValue?: number;
    maxAvgValue?: number;
    minAvgExecTime?: number;
    maxAvgExecTime?: number;
}

export interface UploadResponse {
    message: string;
    data: ResultRecord;
}

export interface ErrorResponse {
    erroro: string;
}