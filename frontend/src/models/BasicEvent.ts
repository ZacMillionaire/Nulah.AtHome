export interface BasicEvent {
	Name: string;
	Id: number;
	Tags: string[];
	Start: Date;
	End?: Date;
}