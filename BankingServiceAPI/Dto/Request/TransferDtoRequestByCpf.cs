namespace BankingServiceAPI.Dto.Request;

public record TransferDtoRequestByCpf(string? OriginCpf, string DestinationCpf, decimal Amount);