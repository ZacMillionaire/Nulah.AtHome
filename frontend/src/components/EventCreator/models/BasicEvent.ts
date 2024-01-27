export interface BasicEvent {
	Description: string;
	Id: number;
	Tags: string[];
	Start: Date;
	End?: Date;
}