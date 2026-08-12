import { Component, ElementRef, ViewChild } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { ResultRecord } from '../../models/models';

@Component({
    selector: 'app-upload',
    templateUrl: './upload.component.html',
    styleUrls: ['./upload.component.css']
})
export class UploadComponent {
    @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
    
    selectedFile: File | null = null;
    uploading = false;
    uploadResult: ResultRecord | null = null;
    errorMessage: string | null = null;
    fileName: string = '';
    dragOver = false;

    constructor(private apiService: ApiService) {
        console.log('UploadComponent initialized');
    }

    onFileSelected(event: any): void {
        console.log('onFileSelected triggered', event);
        const file = event.target.files[0];
        if (file) {
            this.selectedFile = file;
            this.fileName = file.name;
            this.errorMessage = null;
            this.uploadResult = null;
            console.log('File selected:', file.name, file.size, file.type);
        } else {
            console.log('No file selected');
        }
    }

    onUpload(): void {
        console.log('onUpload triggered');
        console.log('Selected file:', this.selectedFile);

        if (!this.selectedFile) {
            this.errorMessage = 'Пожалуйста, выберите файл';
            console.log('No file selected');
            return;
        }

        this.uploading = true;
        this.errorMessage = null;
        this.uploadResult = null;

        console.log('Uploading file:', this.selectedFile.name);

        this.apiService.uploadFile(this.selectedFile).subscribe({
            next: (data: any) => {
                console.log('Upload success:', data);
                this.uploadResult = data.data;
                this.uploading = false;
                this.selectedFile = null;
                this.fileName = '';
                if (this.fileInput) {
                    this.fileInput.nativeElement.value = '';
                }
            },
            error: (error: any) => {
                console.error('Upload error details:', error);
                console.error('Error status:', error.status);
                console.error('Error message:', error.message);
                console.error('Error error:', error.error);

                if (error.status === 0) {
                    this.errorMessage = 'Не удалось соединиться с сервером. Проверьте, запущен ли API на порту 5000.';
                } else if (error.status === 404) {
                    this.errorMessage = 'API эндпоинт не найден. Проверьте URL.';
                } else {
                    this.errorMessage = error.error?.error || error.message || 'Ошибка при загрузке файла';
                }
                this.uploading = false;
            }
        });
    }

    // Метод для форматирования чисел
    formatNumber(value: number): string {
        if (value === undefined || value === null) return '0';
        return value.toFixed(2);
    }

    // Метод для форматирования дат
    formatDate(date: string | Date): string {
        if (!date) return '';
        return new Date(date).toLocaleString('ru-RU', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        });
    }

    // Обработчики Drag & Drop
    onDragOver(event: DragEvent): void {
        event.preventDefault();
        event.stopPropagation();
        this.dragOver = true;
        console.log('Drag over');
    }

    onDragLeave(event: DragEvent): void {
        event.preventDefault();
        event.stopPropagation();
        this.dragOver = false;
        console.log('Drag leave');
    }

    onDragEnter(event: DragEvent): void {
        event.preventDefault();
        event.stopPropagation();
        this.dragOver = true;
        console.log('Drag enter');
    }

    onDrop(event: DragEvent): void {
        event.preventDefault();
        event.stopPropagation();
        this.dragOver = false;

        console.log('Drop event triggered');
        const files = event.dataTransfer?.files;
        console.log('Files dropped:', files?.length);

        if (files && files.length > 0) {
            const file = files[0];
            console.log('File:', file.name, file.type, file.size);

            if (file.type === 'text/csv' || file.name.endsWith('.csv')) {
                this.selectedFile = file;
                this.fileName = file.name;
                this.errorMessage = null;
                this.uploadResult = null;
                console.log('File dropped successfully:', file.name);
            } else {
                this.errorMessage = 'Пожалуйста, загрузите только CSV файлы';
                console.log('Invalid file type:', file.type);
            }
        } else {
            console.log('No files in drop event');
        }
    }
}