import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ResultRecord,ValueRecord } from '../models/models';

@Injectable({
    providedIn: 'root'
})
export class ApiService {
    private apiUrl = '/api';

    constructor(private http: HttpClient) {}

    // 1. Загрузка CSV файла
    uploadFile(file: File): Observable<any> {
        const formData = new FormData();
        formData.append('file', file);
        
        console.log('Uploading file to:', `${this.apiUrl}/data/upload`);
        console.log('File:', file.name, file.size, file.type);
        
        return this.http.post<any>(`${this.apiUrl}/data/upload`, formData);
    }

    // 2. Получение результатов с фильтрами
    getResults(filters: any): Observable<ResultRecord[]> {
        let params = new HttpParams();
        if (filters) {
            Object.keys(filters).forEach(key => {
                const value = filters[key];
                if (value !== undefined && value !== null && value !== '') {
                    params = params.set(key, value.toString());
                }
            });
        }
        console.log('Getting results with params:', params.toString());
        return this.http.get<ResultRecord[]>(`${this.apiUrl}/data/results`, { params });
    }

    // 3. Получение последних 10 значений
    getLast10Values(fileName: string): Observable<ValueRecord[]> {
        console.log('Getting last 10 values for:', fileName);
        return this.http.get<ValueRecord[]>(`${this.apiUrl}/data/values/${fileName}/last10`);
    }
}