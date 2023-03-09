namespace Platform.Geolocation.HostGeolocation.Stun;

public enum StunAttributeType : ushort
{
    /* Comprehension-required range (0x0000-0x7FFF): */
    MAPPED_ADDRESS = 0x0001,
    RESPONSE_ADDRESS = 0x0002, // Reserved [RFC5389]
    CHANGE_REQUEST = 0x0003, // Reserved [RFC5389]
    SOURCE_ADDRESS = 0x0004, // Reserved [RFC5389]
    CHANGED_ADDRESS = 0x0005, // Reserved [RFC5389]
    USERNAME = 0x0006,
    PASSWORD = 0x0007, // Reserved [RFC5389]
    MESSAGE_INTEGRITY = 0x0008,
    ERROR_CODE = 0x0009,
    UNKNOWN_ATTRIBUTES = 0x000A,
    REFLECTED_FROM = 0x000B, // Reserved [RFC5389]
    REALM = 0x0014,
    NONCE = 0x0015,
    XOR_MAPPED_ADDRESS = 0x0020,

    /* Comprehension-optional range (0x8000-0xFFFF) */
    SOFTWARE = 0x8022,
    ALTERNATE_SERVER = 0x8023,
    FINGERPRINT = 0x8028,

    // New attributes
    /* Comprehension-required range (0x0000-0x7FFF): */
    MESSAGE_INTEGRITY_SHA256 = 0x001C,
    PASSWORD_ALGORITHM = 0x001D,
    USERHASH = 0x001E,

    /* Comprehension-optional range (0x8000-0xFFFF) */
    PASSWORD_ALGORITHMS = 0x8002,
    ALTERNATE_DOMAIN = 0x8003,
}